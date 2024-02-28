namespace Battleship.Models;

public class ShootResponse
{
    public int X { get; set; }
    public int Y { get; set; }
    public bool Hit { get; set; }
    public bool Sink { get; set; }
    public Boat SunkBoat { get; set; }
    public bool PlayerWon { get; set; }
    public Position IAShootPosition { get; set; }
    public bool IAShootHit { get; set; }
    public bool IAShootSink { get; set; }
    public Boat IASunkBoat { get; set; }
    public bool IAWon { get; set; }

    public ShootResponse(){}
    
    public override string ToString()
    {
        return $"X: {X}, Y: {Y}, Hit: {Hit}, Sink: {Sink}, SunkBoat: {SunkBoat}, PlayerWon: {PlayerWon}\n" +
               $"IA Shoot Position: {IAShootPosition}, IA Shoot Hit: {IAShootHit}, IA Shoot Sink: {IAShootSink}, IA Sunk Boat: {IASunkBoat}, IA Won: {IAWon}";
    }
}