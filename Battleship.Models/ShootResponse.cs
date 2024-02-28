namespace Battleship.Models;

public class ShootResponse
{
    public int X { get; set; }
    public int Y { get; set; }
    public bool Hit { get; set; }
    public bool Sink { get; set; }
    public int SunkSize { get; set; }
    public bool PlayerWon { get; set; }
    public Position IAShootPosition { get; set; }
    public bool IAShootHit { get; set; }
    public bool IAShootSink { get; set; }
    public int IASunkSize { get; set; }
    public bool IAWon { get; set; }

    public ShootResponse(){}
    
    public override string ToString()
    {
        return $"X: {X}, Y: {Y}, Hit: {Hit}, Sink: {Sink}, SunkSize: {SunkSize}, PlayerWon: {PlayerWon}\n" +
               $"IA Shoot Position: {IAShootPosition}, IA Shoot Hit: {IAShootHit}, IA Shoot Sink: {IAShootSink}, IA Sunk Size: {IASunkSize}, IA Won: {IAWon}";
    }
}