namespace Battleship.Models;

public class Boat
{
    public char Name { get; }
    public int Size { get; }
    public string Facing { get; set; }   //Variable to know the direction of the boat
    public Position Position { get; set; }   //Position X of the first boat grid
    public bool IsSinked { get; set; }

    public Boat(char name, int size)
    {
        this.Name = name;
        this.Size = size;
        this.IsSinked = false;
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
}
