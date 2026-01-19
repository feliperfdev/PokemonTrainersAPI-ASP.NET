using DOTNETPokemonAPI.Database;
using Microsoft.EntityFrameworkCore;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
dataSourceBuilder.EnableDynamicJson();

var dataSource = dataSourceBuilder.Build();

await using var conn = dataSource.OpenConnection();

Console.WriteLine($"PostgreSQL version: {conn.PostgreSqlVersion}");

builder.Services.AddDbContext<PokemonDb>(options =>
    options.UseNpgsql(dataSource));

var app = builder.Build();

app.MapGet("/trainers", async (PokemonDb db) =>
{
    var trainers = await db.Trainers.ToListAsync();

    var allPokemonIds = trainers
        .Where(t => t.PokemonIds != null)
        .SelectMany(t => t.PokemonIds)
        .Distinct()
        .ToList();

    var allPokemons = await db.Pokemons
        .Where(p => allPokemonIds.Contains(p.Id))
        .ToListAsync();

    foreach (var trainer in trainers)
    {
        if (trainer.PokemonIds != null)
        {
            trainer.Pokemons = allPokemons
                .Where(p => trainer.PokemonIds.Contains(p.Id))
                .ToList();
        }
    }

    return trainers.OrderBy((t) => t.Id);
}
     );

app.MapGet("/trainer/{id}", async (int id, PokemonDb db) =>
{
    var trainer = await db.Trainers.FindAsync(id);

    if (trainer is null) return Results.NotFound();

    var allPokemonIds = trainer.PokemonIds.ToList();

    var allPokemons = await db.Pokemons
        .Where(p => allPokemonIds.Contains(p.Id))
        .ToListAsync();

    trainer.Pokemons = allPokemons
                .Where(p => trainer.PokemonIds.Contains(p.Id))
                .ToList();

    return Results.Ok(trainer);
}
     );

app.MapGet("/trainer/{id}/box", async (int id, PokemonDb db) =>
{
    var trainer = await db.Trainers.FindAsync(id);

    if (trainer is null) return Results.NotFound();

    var trainerBox = await db.BoxPCs.FindAsync(trainer.BoxPcId);

    if (trainerBox is null) return Results.NotFound();

    var allPokemonIds = trainerBox.PokemonIds.ToList();

    var allPokemons = await db.Pokemons
        .Where(p => allPokemonIds.Contains(p.Id))
        .ToListAsync();

    trainerBox.Pokemons = allPokemons
                .Where(p => trainerBox.PokemonIds.Contains(p.Id))
                .ToList();

    return Results.Ok(trainerBox);
}
     );

app.Run();
