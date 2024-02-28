namespace Battleship.Models;

public class GridModel
{
    public int Id { get; }
    public Boat[] BoatList { get; set; }
    public char [,] Grid { get; set; }

    public GridModel(int id, int size)
    {
        this.Id = id;
        this.Grid = new char[size, size];
    }
}