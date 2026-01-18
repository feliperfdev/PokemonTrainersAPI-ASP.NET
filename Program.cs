using DOTNETPokemonAPI.Database;
using Microsoft.EntityFrameworkCore;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

await using var conn = new NpgsqlConnection(connectionString);
await conn.OpenAsync();

Console.WriteLine($"PostgreSQL version: {conn.PostgreSqlVersion}");

builder.Services.AddDbContext<PokemonDb>(options =>
    options.UseNpgsql(connectionString));

var app = builder.Build();

app.MapGet("/trainers", async (PokemonDb db) =>
    await db.Trainers.ToListAsync());

app.Run();
