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
                    Trainer = bp.BoxTrainer
                })
                .FirstOrDefaultAsync();

            if (box is null) return Results.NotFound();

            var allTrainersPokemons = await db.TrainerPokemon
                .Where(t => t.TrainerId == box.Trainer!.Id && t.Location == "box")
                .ToListAsync();

            var allPokemons = await db.Pokemons.ToListAsync();

            var trainerPokemonIds = allTrainersPokemons
                    .Select(p => p.PokemonId);

            var dto = new BoxPokemonDTO
            {
                Id = boxId,
                BoxTrainerId = box.Trainer!.Id,
                BoxTrainerName = box.Trainer!.Name,
                Pokemons = [..allPokemons.Where(p => trainerPokemonIds.Contains(p.Id))]
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
            var allTrainerPokemons = await db.TrainerPokemon.ToListAsync();

            var pokemonsInBoxBeingMoved = allTrainerPokemons
                .Where(p => p.TrainerId == trainer.Id && p.Location == "box")
                .Select(p => p.PokemonId)
                .Intersect(paramDTO.PokemonToMoveIds).ToList();
            if (pokemonsInBoxBeingMoved.Count == 0)
                return Results.BadRequest(new { Message = "None of the specified Pokémon are in the box!" });

            var currentPartySize = allTrainerPokemons
                .Where(p => p.TrainerId == trainer.Id && p.Location == "party")
                .Select(p => p.PokemonId)
                .Intersect(paramDTO.PokemonToMoveIds).ToList().Count;
            if (currentPartySize + pokemonsInBoxBeingMoved.Count > PARTYLIMIT)
                return Results.BadRequest(new
                {
                    Message = $"Cannot move {pokemonsInBoxBeingMoved.Count} Pokémon to party. Party limit is {PARTYLIMIT}. Current party size: {currentPartySize}"
                });

            var beingMoved = allTrainerPokemons
                .Where(p => paramDTO.PokemonToMoveIds.Contains(p.PokemonId))
                .Select(p =>
                {
                    p.Location = "party";

                    return p;
                }).ToList();

            db.TrainerPokemon.UpdateRange(beingMoved);
            await db.SaveChangesAsync();

            var transferredPokemons = await db.Pokemons
                .Where(p => pokemonsInBoxBeingMoved.Contains(p.Id))
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
            var allTrainerPokemons = await db.TrainerPokemon.ToListAsync();

            var pokemonsInParty = allTrainerPokemons
                .Where(p => p.TrainerId == trainer.Id && p.Location == "party")
                .Select(p => p.PokemonId).ToList();

            var pokemonsInPartyBeingMoved = pokemonsInParty
                .Intersect(paramDTO.PokemonToMoveIds).ToList();
            if (pokemonsInPartyBeingMoved.Count == 0)
                return Results.BadRequest(new { Message = "None of the specified Pokémon are in the box!" });

            var remainingPartySize = pokemonsInParty.Count - pokemonsInPartyBeingMoved.Count;
            if (remainingPartySize < 1)
                return Results.BadRequest(new { Message = "Cannot move all Pokémon from party. At least 1 must remain!" });

            var beingMoved = allTrainerPokemons
                .Where(p => paramDTO.PokemonToMoveIds.Contains(p.PokemonId))
                .Select(p =>
                {
                    p.Location = "box";

                    return p;
                }).ToList();

            await db.SaveChangesAsync();

            var transferredPokemons = await db.Pokemons
                .Where(p => pokemonsInPartyBeingMoved.Contains(p.Id))
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
