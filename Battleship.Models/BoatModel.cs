namespace Battleship.Models;

public class Boat
{
    public string Name { get; }
    public int Size { get; }
    public string Facing { get; set; }   //Variable to know the direction of the boat
    public int XPos { get; set; }   //Position X of the first boat grid
    public int YPos { get; set; }   //Position Y of the first boat grid

    public Boat(string name, int size)
    {
        this.Name = name;
        this.Size = size;
    }
}
