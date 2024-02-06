namespace Battleship.Models;

public class PlayerModel
{
    public string Name { get; set; }
    public Guid Id { get; }
    public GridModel Grid;
    public int Score { get; set; }
    public int CurrentScore { get; set; }
}