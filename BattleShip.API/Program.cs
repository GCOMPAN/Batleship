using BattleShip.API.Services;
using Battleship.Models;
using System.Text.Json; // Make sure to include this for JSON serialization/deserialization

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<GridService>(); // GridService for managing game logic
builder.Services.AddCors(); // CORS policy for allowing cross-origin requests

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(c => { c.AllowAnyMethod(); c.AllowAnyHeader(); c.AllowAnyOrigin(); });

// Existing game setup route
app.MapGet("/StartGameAI", (GridService gridService, bool playerPlacement, bool hardMode, string userName) =>
{
    Console.WriteLine($"Setting up game for {userName}");
    return gridService.SetupGameIA(playerPlacement, hardMode, userName);
});

// Existing shoot route
app.MapPost("/shoot", (GridService gridService, Position position) =>
{
    Console.WriteLine("Shooting");
    return gridService.Shoot(position);
});

// New leaderboard route
app.MapGet("/leaderboard", () =>
{
    var filePath = "./PlayerWins.json"; // Directly specify, just for a test
    Console.WriteLine($"Looking for PlayerWins.json at {filePath}");

    if (File.Exists(filePath))
    {
        var json = File.ReadAllText(filePath);
        Console.WriteLine($"json = {json}");
        var leaderboard = JsonSerializer.Deserialize<List<LeaderboardEntry>>(json);
        return Results.Ok(leaderboard);
    }
    return Results.NotFound("Leaderboard not found");
});

app.Run();
