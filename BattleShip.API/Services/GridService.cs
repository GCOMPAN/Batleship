using Battleship.Models;

namespace BattleShip.API.Services;

public class GridService
{
    public char[,] generateGrid()
    {
        char[,] grid = new char[10, 10];
        
        var a = new Boat("A", 1);
        var b = new Boat("B", 2);
        var c = new Boat("C", 3);
        var d = new Boat("D", 4);
        
        Boat[] boatList = [a, b, c, d];

        foreach (var boat in boatList)
        {
            Console.WriteLine(boat.Name);
        }

        
        
        return grid;
    }
}
