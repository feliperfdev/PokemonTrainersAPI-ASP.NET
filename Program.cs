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

app.MapGet("/trainers", PokemonDbUsecases.GetAllTrainers);
app.MapGet("/trainer/{id}", PokemonDbUsecases.GetTrainerById);
app.MapGet("/trainer/{id}/box", PokemonDbUsecases.GetTrainerBox);
app.MapGet("/trainer/{id}/all", PokemonDbUsecases.GetTrainerAllPokemon);

app.Run();
