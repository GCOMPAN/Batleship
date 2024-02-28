namespace Battleship.Models;

public class Boat
{
    public char Name { get; }
    public int Size { get; }
    public string Facing { get; set; }   // Variable to know the direction of the boat
    public Position Position { get; set; }   // Position X of the first cell of the boat
    public bool IsSunk { get; set; }

    public Boat(char name, int size)
    {
        this.Name = name;
        this.Size = size;
        this.IsSunk = false;
    }

    public bool IsHit(Position shot)
    {
        if (Facing == "E")
        {
            return shot.Y == Position.Y && shot.X >= Position.X && shot.X < Position.X + Size;
        }
        else 
        {
            return shot.X == Position.X && shot.Y >= Position.Y && shot.Y < Position.Y + Size;
        }
    }
    public override string ToString()
    {
        return $"Boat Name: {Name}, Size: {Size}, Facing: {Facing}, Position: ({Position.X}, {Position.Y}), Is Sunk: {IsSunk}";
    }
}
