using DOTNETPokemonAPI.Database;
using DOTNETPokemonAPI.DTO;
using Microsoft.EntityFrameworkCore;

namespace DOTNETPokemonAPI.Usecases
{
    public class BoxDbUsecases
    {
        public static async Task<IResult> GetBox(int boxId, PokemonDb db)
        {
            var box = await db.BoxPCs
                .Include(bp => bp.BoxTrainer)
                .Where(bp => bp.Id == boxId)
                .Select(bp => new
                {
                    BoxPc = bp,
                    Trainer = bp.BoxTrainer,
                    Pokemons = db.Pokemons
                        .Where(p => bp.PokemonIds.Contains(p.Id))
                        .OrderBy(p => p.Id)
                        .ToList()
                })
                .FirstOrDefaultAsync();

            if (box is null) return Results.NotFound();

            var boxPc = box.BoxPc;

            if (boxPc is null) return Results.NotFound();

            var boxTrainer = boxPc.BoxTrainer;

            if (boxTrainer is null) return Results.NotFound();

            var dto = new BoxPokemonDTO
            {
                Id = boxPc.Id,
                BoxTrainerId = boxTrainer.Id,
                BoxTrainerName = boxTrainer.Name,
                Pokemons = box.Pokemons
            };

            return Results.Ok(dto);
        }

        public static async Task<IResult> MovePokemonFromBoxToParty(int boxId, PokemonFromBoxToMoveDTO paramDTO, PokemonDb db)
        {
            var trainer = await db.Trainers.FindAsync(paramDTO.TrainerId);

            if (trainer is null) return Results.NotFound();

            var box = await db.BoxPCs.FindAsync(boxId);

            if (box is null) return Results.NotFound();

            var allPokemonIds = trainer.PokemonIds.ToList();

            var allPokemons = await db.Pokemons
                .Where(p => allPokemonIds.Contains(p.Id))
                .ToListAsync();

            foreach (var pokemonId in paramDTO.PokemonToMoveIds)
            {
                box.PokemonIds.Remove(pokemonId);
                box.Pokemons!.Remove(allPokemons.Where((p) => p.Id == pokemonId).Single());
            }

            db.BoxPCs.Update(box);

            trainer.PokemonIds.AddRange(paramDTO.PokemonToMoveIds);
            trainer.Pokemons.AddRange(allPokemons.Where(pokemon => paramDTO.PokemonToMoveIds.Contains(pokemon.Id)));

            db.Trainers.Update(trainer);

            db.SaveChanges();

            return Results.Accepted();
        }
    }
}
