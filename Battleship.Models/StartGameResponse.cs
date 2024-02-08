namespace Battleship.Models;

public class StartGameAIResponse
{
    public int GameId { get; set; }
    public int PlayerId { get; set; }
    public Boat[] BoatList { get; set; }
}