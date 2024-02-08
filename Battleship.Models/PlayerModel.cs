namespace Battleship.Models;

public class PlayerModel
{
    public string Name { get; set; }
    public int Id { get; }
    public GridModel GridModel;
    public int Score { get; set; }
    public int CurrentScore { get; set; }

    public PlayerModel(string name, int id, GridModel grid)
    {
        this.Name = name;
        this.Id = id;
        this.GridModel = grid;
    }
}