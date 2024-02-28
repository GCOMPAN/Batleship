namespace Battleship.Models;

public class Position
{
    public int X { get; set; }
    public int Y { get; set; }

    public Position(int x, int y)
    {
        this.X = x;
        this.Y = y;
    }
    
    public override string ToString()
    {
        return $"({X}, {Y})";
    }
}