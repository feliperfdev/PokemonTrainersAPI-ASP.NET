using Microsoft.EntityFrameworkCore;

namespace DOTNETPokemonAPI.Database
{
    public class PokemonDbUsecases
    {
        public static async Task<IResult> GetAllTrainers(PokemonDb db)
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

            return Results.Ok(trainers.OrderBy((t) => t.Id));
        }

        public static async Task<IResult> GetTrainerById(int id, PokemonDb db)
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

            return Results.Ok(trainerBox);
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
