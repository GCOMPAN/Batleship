using Battleship.Models;

namespace BattleShip.API.Services;

public class GridService
{
    public Boat[] GenerateBoatsPos()
    {
        char[,] grid = new char[10, 10];
        
        var a = new Boat("A", 1);
        var b = new Boat("B", 2);
        var c = new Boat("C", 3);
        var d = new Boat("D", 4);
        
        Boat[] boatList = [a, b, c, d];

        var random = new Random();
        foreach (var boat in boatList)
        {
            var value = random.Next(2);
            boat.Facing = value == 0 ? "S" : "E";   //S = Sud and E = East

            int maxX = boat.Facing == "S" ? 10 : 10 - boat.Size;
            int maxY = boat.Facing == "E" ? 10 : 10 - boat.Size;

            int rdmXPos = random.Next(maxX);
            boat.XPos = rdmXPos;
            int rdmYPos = random.Next(maxY);
            boat.YPos = rdmYPos;
        }
        return boatList;
    }
}
