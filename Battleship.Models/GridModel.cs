namespace Battleship.Models;

public class GridModel
{
    public int Id { get; }
    public Boat[] BoatList { get; set; }
    public char [,] Grid { get; set; }

    public GridModel(int id)
    {
        this.Id = id;
        this.Grid = new char[10, 10];
    }
}