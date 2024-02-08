using BattleShip.API.Services;
using Battleship.Models;

var builder = WebApplication.CreateBuilder(args);

// Ajoutez les services au conteneur.
// En savoir plus sur la configuration de Swagger/OpenAPI à https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<GridService>();
builder.Services.AddCors();

var app = builder.Build();

// Configurez le pipeline de requêtes HTTP.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/StartGameAI", (GridService gridService) =>
{
    Console.WriteLine("Setuping game");
    return gridService.SetupGameIA();
});

app.MapPost("/shoot", (GridService gridService, Position position)   =>
    {
        Console.WriteLine("Shooting");
        return gridService.Shoot(position);
    });

app.Run();