using DOTNETPokemonAPI.Database;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Text.Json.Serialization;

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

app.Run();
