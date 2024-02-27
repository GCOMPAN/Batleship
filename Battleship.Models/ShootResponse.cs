namespace Battleship.Models;

public class ShootResponse
{
    public int X { get; set; }
    public int Y { get; set; }
    public bool Hit { get; set; }
    public bool Sink { get; set; }
    public bool PlayerWon { get; set; }
    public Position IAShootPosition { get; set; }
    public bool IAShootHit { get; set; }
    public bool IAShootSink { get; set; }
    public bool IAWon { get; set; }

    public ShootResponse(){}
}