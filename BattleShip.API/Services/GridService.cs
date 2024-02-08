using Battleship.Models;

namespace BattleShip.API.Services;

public class GridService
{
    private PlayerModel player1;
    private PlayerModel player2;
    private Position[] IAMovesOrder;
    private int IAIndexMove = 0;
    public bool IsBoatFittingInGrid(Boat boat, GridModel grid)
    {
        var incX = boat.Facing == "E";
        for (int i = 0; i < boat.Size; i++)
        {
            if (incX)
            {
                if (grid.Grid[boat.Position.X + i,boat.Position.Y] != '\0')
                {
                    return false;
                }
            }
            else
            {
                if (grid.Grid[boat.Position.X,boat.Position.Y + i] != '\0')
                {
                    return false;
                }
            }
        }
        return true;
    }

    public void SetBoatOnGrid(Boat boat, GridModel grid)
    {
        var incX = boat.Facing == "E";
        for (int i = 0; i < boat.Size; i++)
        {
            if (incX)
            {
                grid.Grid[boat.Position.X + i, boat.Position.Y] = boat.Name;
            }
            else
            {
                grid.Grid[boat.Position.X, boat.Position.Y + i] = boat.Name;
            }
        }
    }

    public Boat[] GenerateBoatsPos(GridModel grid)
    {
        var a = new Boat('A', 1);
        var b = new Boat('B', 2);
        var c = new Boat('C', 3);
        var d = new Boat('D', 4);
        var e = new Boat('E', 4);
        var f = new Boat('F', 4);
        var g = new Boat('G', 4);
        
        Boat[] boatList = [a, b, c, d, e, f, g];

        var random = new Random();
        foreach (var boat in boatList)
        {
            var value = random.Next(2);
            boat.Facing = value == 0 ? "S" : "E";   //S = South and E = East

            int maxX = boat.Facing == "S" ? 10 : 10 - boat.Size;
            int maxY = boat.Facing == "E" ? 10 : 10 - boat.Size;

            int rdmXPos = random.Next(maxX);
            int rdmYPos = random.Next(maxY);
            Position pos = new(rdmXPos, rdmYPos);
            boat.Position = pos;
            
            while (!IsBoatFittingInGrid(boat, grid))
            {
                rdmXPos = random.Next(maxX);
                boat.Position.X = rdmXPos;
                rdmYPos = random.Next(maxY);
                boat.Position.Y = rdmYPos;
            }
            SetBoatOnGrid(boat, grid);
        }
        return boatList;
    }

    public Position[] GenerateAIMoves()
    {
        var moves = new List<Position>();

        // Generate all possible moves in a 10x10 grid
        for (int x = 0; x < 10; x++)
        {
            for (int y = 0; y < 10; y++)
            {
                moves.Add(new Position(x, y));
            }
        }

        // Randomize the list of moves
        Random rng = new Random();
        int n = moves.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            Position value = moves[k];
            moves[k] = moves[n];
            moves[n] = value;
        }

        return moves.ToArray();
    }


    public Boat[] SetupGameIA()
    {
        GridModel grid1 = new (0);
        grid1.BoatList = GenerateBoatsPos(grid1);
        player1 = new("p1", 0, grid1);

        GridModel grid2 = new (1);
        grid2.BoatList = GenerateBoatsPos(grid2);
        player2 = new("p2", 1, grid2);

        IAMovesOrder = GenerateAIMoves();

        return player1.GridModel.BoatList;
    }

    public (bool isHit, Boat hitBoat) IsHittingShip(Position position, PlayerModel target)
    {
        foreach (var boat in target.GridModel.BoatList)
        {
            if (boat.IsHit(position))
            {
                return(true, boat);
            }
        }

        return (false, null);
    }

    public Position[] GetBoatPositions(Boat boat)
    {
        Position[] positions = [boat.Position];
        
        for (int i = 1; i < boat.Size; i++)
        {
            if (boat.Facing == "E")
            {
                positions.Append(new Position(boat.Position.X + i, boat.Position.Y));
            }
            else
            {
                positions.Append(new Position(boat.Position.X, boat.Position.Y + i));
            }
        }
        
        return positions;
    }
    
    public bool IsBoatSinked(Boat boat, PlayerModel player)
    {
        Position[] boatPositions = GetBoatPositions(boat);
        foreach (var pos in boatPositions)
        {
            if (player.GridModel.Grid[pos.X, pos.Y] != 'X') return false;
        }

        return true;
    }

    public bool IsWinning(PlayerModel player)
    {
        PlayerModel oponent;
        if (player.Name == "p1")
        {
            oponent = player2;
        }
        else
        {
            oponent = player1;
        }

        foreach (var boat in oponent.GridModel.BoatList)
        {
            if (!boat.IsSinked) return false;
        }

        return true;
    }

    public ShootResponse Shoot(Position position)
    {
        // Handle player shot
        ShootResponse response = new();
        var (isHit, boat) = IsHittingShip(position, player2);
        response.Hit = isHit;
        if (isHit)
        {
            player2.GridModel.Grid[position.X, position.Y] = 'X';
            bool isSinking = IsBoatSinked(boat, player2);
            response.Sink = isSinking;
            if (isSinking)
            {
                boat.IsSinked = true;
                bool isWinning = IsWinning(player1);
                response.PlayerWon = isWinning;
                if (isWinning) return response;
            }
        }
        // Handle IA shot
        Position IAShot = IAMovesOrder[IAIndexMove];
        IAIndexMove += 1;
        response.IAShootPosition = IAShot;

        var (isHitIA, boatIA) = IsHittingShip(IAShot, player1);
        response.IAShootHit = isHitIA;
        if (isHitIA)
        {
            player1.GridModel.Grid[IAShot.X, IAShot.Y] = 'X';
            bool isSinkingIA = IsBoatSinked(boatIA, player1);
            response.IAShootSink = isSinkingIA;
            if (isSinkingIA)
            {
                boatIA.IsSinked = true;
                bool isWinningIA = IsWinning(player2);
                response.IAWon = isWinningIA;
                if (isWinningIA) return response;
            }
        }
        return response;
    }
}
