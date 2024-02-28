using Battleship.Models;

namespace BattleShip.API.Services;


public class GridService
{
    private const int GridSize = 10;
    private const char EmptyCell = '\0';
    private const char HitMarker = 'X';

    
    private PlayerModel player1;
    private PlayerModel player2;
    private Position[] IAMovesOrder;
    private int IAIndexMove = 0;
    bool IsBoatFittingInGrid(Boat boat, GridModel grid) {
        int maxX = boat.Facing == "E" ? GridSize - boat.Size : GridSize - 1;
        int maxY = boat.Facing == "S" ? GridSize - boat.Size : GridSize - 1;

        // Check if the boat's starting position is out of bounds
        if (boat.Position.X > maxX || boat.Position.Y > maxY) {
            return false;
        }

        // Check for overlaps
        for (int i = 0; i < boat.Size; i++) {
            int checkX = boat.Position.X + (boat.Facing == "E" ? i : 0);
            int checkY = boat.Position.Y + (boat.Facing == "S" ? i : 0);

            if (grid.Grid[checkX, checkY] != EmptyCell) {
                return false; // Overlap detected
            }
        }

        return true; // The boat fits without overlapping and within bounds
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
        var boats = new Boat[]
        {
            new Boat('A', 1),
            new Boat('B', 2),
            new Boat('C', 3),
            new Boat('D', 4),
            new Boat('E', 4),
            new Boat('F', 4),
            new Boat('G', 4),
        };
        
        var random = new Random();
        foreach (var boat in boats)
        {
            bool fits = false;
            while (!fits)
            {
                var value = random.Next(2);
                boat.Facing = value == 0 ? "S" : "E";

                int maxX = boat.Facing == "S" ? GridSize : GridSize - boat.Size + 1;
                int maxY = boat.Facing == "E" ? GridSize : GridSize - boat.Size + 1;

                int rdmXPos = random.Next(maxX);
                int rdmYPos = random.Next(maxY);
                boat.Position = new Position(rdmXPos, rdmYPos);

                fits = IsBoatFittingInGrid(boat, grid);
            }
            SetBoatOnGrid(boat, grid);
        }
        return boats;
    }


    public Position[] GenerateAIMoves()
    {
        var moves = new List<Position>();

        // Generate all possible moves in a 10x10 grid
        for (int x = 0; x < GridSize; x++)
        {
            for (int y = 0; y < GridSize; y++)
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


    public StartGameAIResponse SetupGameIA()
    {
        var response = new StartGameAIResponse();
        GridModel grid1 = new (0);
        grid1.BoatList = GenerateBoatsPos(grid1);
        player1 = new("p1", 0, grid1);

        GridModel grid2 = new (1);
        grid2.BoatList = GenerateBoatsPos(grid2);
        player2 = new("p2", 1, grid2);

        IAMovesOrder = GenerateAIMoves();

        response.BoatList = player1.GridModel.BoatList;
        response.BoatList2 = player2.GridModel.BoatList;
        response.GameId = 0;
        response.PlayerId = 0;

        return response;
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
        // Initialize a list to hold positions
        List<Position> positions = new List<Position>();

        // Add the starting position
        positions.Add(boat.Position);

        // Loop to add the rest of the positions based on the boat's size
        for (int i = 1; i < boat.Size; i++)
        {
            if (boat.Facing == "E")
            {
                positions.Add(new Position(boat.Position.X + i, boat.Position.Y));
            }
            else // Assuming the only other direction is South ("S")
            {
                positions.Add(new Position(boat.Position.X, boat.Position.Y + i));
            }
        }

        // Convert the list back to an array before returning
        return positions.ToArray();
    }
    
    public bool IsBoatSunk(Boat boat, PlayerModel player)
    {
        Position[] boatPositions = GetBoatPositions(boat);
        foreach (var pos in boatPositions)
        {
            if (player.GridModel.Grid[pos.X, pos.Y] != HitMarker) return false;
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
        response.X = position.X;
        response.Y = position.Y;
        var (isHit, boat) = IsHittingShip(position, player2);
        response.Hit = isHit;
        if (isHit)
        {
            player2.GridModel.Grid[position.X, position.Y] = HitMarker;
            bool isSinking = IsBoatSunk(boat, player2);
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
            player1.GridModel.Grid[IAShot.X, IAShot.Y] = HitMarker;
            bool isSinkingIA = IsBoatSunk(boatIA, player1);
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
