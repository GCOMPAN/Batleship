using Battleship.Models;

namespace BattleShip.API.Services;

public class GridService
{
    private const int GridSize = 10;
    private const char EmptyCell = '\0';
    private const char HitMarker = 'X';
    private const char MissMarker = 'O';

    
    private PlayerModel player1;
    private PlayerModel player2;
    private Position[] IAMovesOrder;
    private int IAIndexMove = 0;
    private List<IAHistoryModel> IAHistory = new List<IAHistoryModel>();
    private Random rng = new Random();
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

    public List<IAHistoryModel> GetReversedHittingPositions()
    {
        // Filter the IAHistory to include only the shots where IsHitting is true
        var hittingShots = IAHistory.Where(shot => shot.IsHitting).ToList();

        // Reverse the list to have the most recent hits first
        hittingShots.Reverse();

        return hittingShots;
    }
    
    private List<Position> GetAdjacentPositions(Position position)
    {
        // Assuming Position has X and Y as its properties
        List<Position> positions = new List<Position>
        {
            new Position(position.X + 1, position.Y),
            new Position(position.X - 1, position.Y),
            new Position(position.X, position.Y + 1),
            new Position(position.X, position.Y - 1)
        };

        // Ensure positions are within the grid
        return positions.Where(p => p.X >= 0 && p.X < GridSize && p.Y >= 0 && p.Y < GridSize).ToList();
    }
    
    private bool IsValidMove(Position pos)
    {
        return player1.GridModel.Grid[pos.X, pos.Y] != HitMarker && player1.GridModel.Grid[pos.X, pos.Y] != MissMarker;
    }
    
    private Position GetRandomValidPosition()
    {
        Position randomPosition;
        do
        {
            randomPosition = new Position(rng.Next(GridSize), rng.Next(GridSize));
        } while (!IsValidMove(randomPosition));

        return randomPosition;
    }

    public Position nextIAMove()
    {
        // If no moves have been made, choose a random position
        if (IAHistory.Count == 0)
        {
            return new Position(rng.Next(GridSize), rng.Next(GridSize));
        }
        else
        {
            // Retrieve the reversed list of hitting positions, including only those not marked as ignored
            var reversedHittingPositions = GetReversedHittingPositions()
                .Where(hit => !hit.Ignore) // Filter out the hits that should be ignored
                .ToList();

            foreach (var hit in reversedHittingPositions)
            {
                // For each hit, get potential adjacent positions that haven't been hit or missed
                List<Position> possibleMoves = GetAdjacentPositions(hit.Position)
                    .Where(pos => player1.GridModel.Grid[pos.X, pos.Y] != HitMarker 
                                  && player1.GridModel.Grid[pos.X, pos.Y] != MissMarker)
                    .ToList();

                // If there are valid adjacent positions, select one randomly
                if (possibleMoves.Any())
                {
                    return possibleMoves[rng.Next(possibleMoves.Count)];
                }
            }

            // If no valid moves are found next to any hits, or all hits are processed, select a random new position
            return GetRandomValidPosition();
        }
    }

    public StartGameAIResponse SetupGameIA(bool playerPlacement)
    {
        var response = new StartGameAIResponse();
        GridModel grid1 = new (0);
        Console.WriteLine($"Result of bool: {playerPlacement}");
        player1 = null;
        player2 = null;
        
        if (playerPlacement)
        {
            grid1.BoatList = GenerateBoatsPos(grid1);
            player1 = new("p1", 0, grid1);
        
        
            GridModel grid2 = new (1);
            grid2.BoatList = GenerateBoatsPos(grid2);
            player2 = new("p2", 1, grid2);
        }
        IAMovesOrder = GenerateAIMoves();

        response.BoatList = player1.GridModel.BoatList;
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

    public void ManageIAHistory()
    {
        // Retrieve the reversed list of hitting positions
        var hitList = GetReversedHittingPositions();

        // Find the last sinking event
        var lastSinkingEvent = hitList.FirstOrDefault(hit => hit.IsSinking);

        if (lastSinkingEvent != null)
        {
            // Get the size of the sunk boat from the last sinking event
            int sunkBoatSize = lastSinkingEvent.Size;

            // Count of hits to be marked as ignored
            int hitsToIgnore = sunkBoatSize;

            // Iterate over the hitList to set Ignore flag for the sunk boat's hits
            foreach (var hit in hitList)
            {
                if (hitsToIgnore > 0 && hit.IsHitting)
                {
                    hit.Ignore = true;
                    hitsToIgnore--;
                }
                // Once we've marked enough hits based on the sunk boat size, break out of the loop
                if (hitsToIgnore == 0) break;
            }
        }
    }


    public ShootResponse Shoot(Position position)
    {
        // Handle player shot
        ShootResponse response = new();
        response.X = position.X;
        response.Y = position.Y;
        var (isHit, boat) = IsHittingShip(position, player2);
        response.Hit = isHit;
        player2.GridModel.Grid[position.X, position.Y] = isHit ? HitMarker : MissMarker;
        if (isHit)
        {
            bool isSinking = IsBoatSunk(boat, player2);
            response.Sink = isSinking;
            if (isSinking)
            {
                response.SunkSize = boat.Size;
                boat.IsSinked = true;
                bool isWinning = IsWinning(player1);
                response.PlayerWon = isWinning;
                if (isWinning) return response;
            }
        }
        // Handle IA shot
        Position IAShot = nextIAMove();
        IAIndexMove += 1;
        response.IAShootPosition = IAShot;

        var (isHitIA, boatIA) = IsHittingShip(IAShot, player1);
        response.IAShootHit = isHitIA;
        player1.GridModel.Grid[IAShot.X, IAShot.Y] = isHitIA ? HitMarker : MissMarker;
        if (isHitIA)
        {
            bool isSinkingIA = IsBoatSunk(boatIA, player1);
            response.IAShootSink = isSinkingIA;
            if (isSinkingIA)
            {
                response.IASunkSize = boatIA.Size;
                boatIA.IsSinked = true;
                bool isWinningIA = IsWinning(player2);
                response.IAWon = isWinningIA;
                if (isWinningIA) return response;
            }
        }

        IAHistory.Add(new IAHistoryModel
        {
            Position = response.IAShootPosition,
            IsHitting = response.IAShootHit,
            IsSinking = response.IAShootSink,
            Size = response.IASunkSize
        });
        if (response.IAShootSink) ManageIAHistory();
        return response;
    }
}
