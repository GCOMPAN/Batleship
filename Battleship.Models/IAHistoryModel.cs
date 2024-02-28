namespace Battleship.Models;

public class IAHistoryModel
{
    public Position Position { get; set; }
    public bool IsHitting { get; set; }
    public bool IsSinking { get; set; }
    
    public int Size { get; set; }
    public bool Ignore { get; set; }
}