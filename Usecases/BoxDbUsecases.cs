using DOTNETPokemonAPI.Database;
using DOTNETPokemonAPI.DTO;
using DOTNETPokemonAPI.Exceptions;
using DOTNETPokemonAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DOTNETPokemonAPI.Usecases
{
    public class BoxDbUsecases
    {

        private const int PARTYLIMIT = 6;

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

        public static async Task<IResult> MovePokemon(
            int boxId, 
            [FromBody] PokemonFromBoxToMoveDTO paramDTO, 
            PokemonDb db)
        {
            var trainer = await db.Trainers.FindAsync(paramDTO.TrainerId);

            if (trainer is null) return Results.NotFound(
                new NotFoundException
                {
                    Message = $"Trainer [id = {paramDTO.TrainerId}] not found!"
                }
                );

            if (boxId != trainer.BoxPcId)
                return Results.BadRequest(new { Message = "Box does not belong to this trainer!" });

            var box = await db.BoxPCs.FindAsync(boxId);

            if (box is null) return Results.NotFound(
                new NotFoundException
                {
                    Message = $"Box [id = {boxId}] not found!"
                }
                );

            return paramDTO.Type switch
            {
                MoveTypeEnum.BoxToParty => await MoveFromBoxToParty(box, trainer, paramDTO, db),
                MoveTypeEnum.PartyToBox => await MoveFromPartyToBox(box, trainer, paramDTO, db),
                _ => Results.BadRequest(new { Message = "Invalid move type!" })
            };
        }
        
        private static async Task<IResult> MoveFromBoxToParty(
            BoxPC box, Trainer trainer,
            PokemonFromBoxToMoveDTO paramDTO,
            PokemonDb db
            )
        {
            var pokemonsInBox = box.PokemonIds.Intersect(paramDTO.PokemonToMoveIds).ToList();
            if (pokemonsInBox.Count == 0)
                return Results.BadRequest(new { Message = "None of the specified Pokémon are in the box!" });

            var currentPartySize = trainer.PokemonIds.Count;
            if (currentPartySize + pokemonsInBox.Count > PARTYLIMIT)
                return Results.BadRequest(new
                {
                    Message = $"Cannot move {pokemonsInBox.Count} Pokémon to party. Party limit is {PARTYLIMIT}. Current party size: {currentPartySize}"
                });

            box.PokemonIds = [.. box.PokemonIds.Except(pokemonsInBox)];
            trainer.PokemonIds = [.. trainer.PokemonIds.Concat(pokemonsInBox)];

            db.BoxPCs.Update(box);
            db.Trainers.Update(trainer);
            await db.SaveChangesAsync();

            var transferredPokemons = await db.Pokemons
                .Where(p => pokemonsInBox.Contains(p.Id))
                .ToListAsync();

            return Results.Ok(
                new TransferedPokemonDTO
                {
                    TrainerId = trainer.Id,
                    BoxId = box.Id,
                    TransferedToBox = [],
                    TransferedToParty = transferredPokemons,
                }
                );
        }

        private static async Task<IResult> MoveFromPartyToBox(
            BoxPC box, Trainer trainer,
            PokemonFromBoxToMoveDTO paramDTO,
            PokemonDb db
            )
        {
            var pokemonsInParty = trainer.PokemonIds.Intersect(paramDTO.PokemonToMoveIds).ToList();
            if (pokemonsInParty.Count == 0)
                return Results.BadRequest(new { Message = "None of the specified Pokémon are in the party!" });

            var remainingPartySize = trainer.PokemonIds.Count - pokemonsInParty.Count;
            if (remainingPartySize < 1)
                return Results.BadRequest(new { Message = "Cannot move all Pokémon from party. At least 1 must remain!" });

            trainer.PokemonIds = [.. trainer.PokemonIds.Except(pokemonsInParty)];

            box.PokemonIds = [.. box.PokemonIds.Concat(pokemonsInParty)];

            db.Trainers.Update(trainer);
            db.BoxPCs.Update(box);
            await db.SaveChangesAsync();

            var transferredPokemons = await db.Pokemons
                .Where(p => pokemonsInParty.Contains(p.Id))
                .ToListAsync();

            return Results.Ok(
                new TransferedPokemonDTO
                {
                    TrainerId = trainer.Id,
                    BoxId = box.Id,
                    TransferedToBox = transferredPokemons,
                    TransferedToParty = [],
                }
                );
        }
    }
}
