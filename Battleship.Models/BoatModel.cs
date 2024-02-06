namespace Battleship.Models;

public class Boat
{
    public string Name;
    public int Size;
    public string Facing { get; set; }
    public int XPos { get; set; }
    public int YPos { get; set; }

    public Boat(string name, int size)
    {
        this.Name = name;
        this.Size = size;
    }
}
