using DOTNETPokemonAPI.Database;
using DOTNETPokemonAPI.DTO;
using DOTNETPokemonAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DOTNETPokemonAPI.Usecases
{
    public class PokemonDbUsecases
    {
        public static async Task<IResult> GetAllPokemon(PokemonDb db)
        {
            var pokemons = await db.Pokemons.ToListAsync();

            if (pokemons is null) return Results.NotFound();

            return Results.Ok(pokemons.OrderBy((p) => p.Id));
        }

        public static async Task<IResult> GetAllTrainers(PokemonDb db)
        {
            var trainers = await db.Trainers.ToListAsync();

            var allTrainersPokemons = await db.TrainerPokemon.ToListAsync();

            var trainersWithPokemon = new List<TrainerWithPokemonDTO>();

            var allPokemons = await db.Pokemons.ToListAsync();

            foreach (var trainer in trainers)
            {
                var trainerPokemons = allTrainersPokemons
                    .Where(p => p.TrainerId == trainer.Id && p.Location == "party")
                    .Select(p => p.PokemonId); ;

                var dto = new TrainerWithPokemonDTO
                {
                    Id = trainer.Id,
                    BoxId = trainer.BoxPcId!.Value,
                    Pokemons = [.. allTrainersPokemons.Select(p => {
                    var pokemon = allPokemons.Where(pkmn => 
                    allPokemons.Select(p => p.Id).Contains(p.PokemonId)).First();

                    p.Name = pokemon.Name;

                    return p;
                })]
                };

                trainersWithPokemon.Add(dto);
            }

            return Results.Ok(trainersWithPokemon.OrderBy((t) => t.Id));
        }

        public static async Task<IResult> GetTrainerById(int id, PokemonDb db)
        {
            var trainer = await db.Trainers.FindAsync(id);

            if (trainer is null) return Results.NotFound();

            var allTrainersPokemons = await db.TrainerPokemon
                .Where(t => t.TrainerId == id && t.Location == "party")
                .ToListAsync();

            var allPokemons = await db.Pokemons.ToListAsync();

            var trainerPokemonIds = allTrainersPokemons
                    .Select(p => p.PokemonId);

            var dto = new TrainerWithPokemonDTO
            {
                Id = trainer.Id,
                BoxId = trainer.BoxPcId!.Value,
                Pokemons = [.. allTrainersPokemons.Select(p => {
                    var pokemon = allPokemons.Where(pkmn => trainerPokemonIds.Contains(p.PokemonId)).First();

                    p.Name = pokemon.Name;

                    return p;
                })]
            };

            return Results.Ok(dto);
        }

        public static async Task<IResult> GetTrainerBox(int id, PokemonDb db)
        {
            var trainer = await db.Trainers.FindAsync(id);

            if (trainer is null) return Results.NotFound();

            var allTrainersPokemons = await db.TrainerPokemon
                .Where(t => t.TrainerId == id && t.Location == "box")
                .ToListAsync();

            var allPokemons = await db.Pokemons.ToListAsync();

            var trainerPokemonIds = allTrainersPokemons
                    .Select(p => p.PokemonId); ;

            var dto = new TrainerWithPokemonDTO
            {
                Id = trainer.Id,
                BoxId = trainer.BoxPcId!.Value,
                Pokemons = [.. allTrainersPokemons.Select(p => {
                    var pokemon = allPokemons.Where(pkmn => trainerPokemonIds.Contains(p.PokemonId)).First();

                    p.Name = pokemon.Name;

                    return p;
                })]
            };

            return Results.Ok(dto);
        }

        public static async Task<IResult> GetTrainerAllPokemon(int id, PokemonDb db)
        {
            var trainer = await db.Trainers.FindAsync(id);

            if (trainer is null) return Results.NotFound();

            var allTrainersPokemons = await db.TrainerPokemon
                .Where(t => t.TrainerId == id)
                .ToListAsync();

            var allPokemons = await db.Pokemons.ToListAsync();

            var trainerPokemonIds = allTrainersPokemons
                    .Select(p => p.PokemonId); ;

            var dto = new TrainerWithPokemonDTO
            {
                Id = trainer.Id,
                BoxId = trainer.BoxPcId!.Value,
                Pokemons = [.. allTrainersPokemons.Select(p => {
                    var pokemon = allPokemons.Where(pkmn => trainerPokemonIds.Contains(p.PokemonId)).First();

                    p.Name = pokemon.Name;

                    return p;
                })]
            };

            return Results.Ok(dto);
        }
    }
}
