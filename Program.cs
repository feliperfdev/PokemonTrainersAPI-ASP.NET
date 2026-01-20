using DOTNETPokemonAPI.Database;
using DOTNETPokemonAPI.Usecases;
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

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.MapGet("/pokemons", PokemonDbUsecases.GetAllPokemon);
app.MapGet("/trainers", PokemonDbUsecases.GetAllTrainers);
app.MapGet("/trainer/{id}", PokemonDbUsecases.GetTrainerById);
app.MapGet("/trainer/{id}/box", PokemonDbUsecases.GetTrainerBox);
app.MapGet("/trainer/{id}/all", PokemonDbUsecases.GetTrainerAllPokemon);

app.MapGet("/box/{boxId}", BoxDbUsecases.GetBox);
app.MapPut("/box/{boxId}/move", BoxDbUsecases.MovePokemon);

app.Run();
