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

            var trainersWithPokemon = new List<TrainerWithPokemonDTO>();

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
                    var dto = new TrainerWithPokemonDTO
                    {
                        Id = trainer.Id,
                        BoxId = trainer.BoxPcId!.Value,
                        Pokemons = allPokemons
                        .Where(p => trainer.PokemonIds.Contains(p.Id))
                        .ToList()
                    };
                    
                    trainersWithPokemon.Add(dto);
                }
            }

            return Results.Ok(trainersWithPokemon.OrderBy((t) => t.Id));
        }

        public static async Task<IResult> GetTrainerById(int id, PokemonDb db)
        {
            var trainer = await db.Trainers.FindAsync(id);

            if (trainer is null) return Results.NotFound();

            var allPokemonIds = trainer.PokemonIds.ToList();

            var allPokemons = await db.Pokemons
                .Where(p => allPokemonIds.Contains(p.Id))
                .ToListAsync();

            var dto = new TrainerWithPokemonDTO
            {
                Id = trainer.Id,
                BoxId = trainer.BoxPcId!.Value,
                Pokemons = allPokemons
                        .Where(p => trainer.PokemonIds.Contains(p.Id))
                        .ToList()
            };

            return Results.Ok(dto);
        }

        public static async Task<IResult> GetTrainerBox(int id, PokemonDb db)
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

            var dto = new BoxPokemonDTO
            {
                Id = trainerBox.Id,
                BoxTrainerId = trainer.Id,
                BoxTrainerName = trainer.Name,
                Pokemons = trainerBox.Pokemons
            };

            return Results.Ok(dto);
        }

        public static async Task<IResult> GetTrainerAllPokemon(int id, PokemonDb db)
        {
            var trainer = await db.Trainers.FindAsync(id);

            if (trainer is null) return Results.NotFound();

            var trainerBox = await db.BoxPCs.FindAsync(trainer.BoxPcId);

            if (trainerBox is null) return Results.NotFound();

            var allPokemonIds = trainerBox.PokemonIds.Concat(trainer.PokemonIds).ToList();

            var allPokemons = await db.Pokemons
                .Where(p => allPokemonIds.Contains(p.Id))
                .ToListAsync();

            trainerBox.Pokemons = allPokemons
                        .Where(p => trainerBox.PokemonIds.Contains(p.Id))
                        .ToList();

            trainer.Pokemons = allPokemons
                        .Where(p => trainer.PokemonIds.Contains(p.Id))
                        .ToList();

            var dto = new TrainerAllPokemonDTO { 
            Id = trainer.Id,
            Name = trainer.Name,
            };

            dto.AddPokemon(partyPokemon: trainer.Pokemons, boxPokemon: trainerBox.Pokemons);

            return Results.Ok(dto);
        }
    }
}
