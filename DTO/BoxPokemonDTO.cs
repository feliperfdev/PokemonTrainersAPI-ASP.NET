using DOTNETPokemonAPI.Models;

namespace DOTNETPokemonAPI.DTO
{
    public class BoxPokemonDTO
    {
        public required int Id { get; set; }

        public required int BoxTrainerId { get; set; }

        public required String BoxTrainerName { get; set; }

        public List<TrainerPokemon>? Pokemons { get; set; }

    }
}
