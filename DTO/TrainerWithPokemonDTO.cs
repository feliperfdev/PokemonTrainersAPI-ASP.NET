using DOTNETPokemonAPI.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace DOTNETPokemonAPI.DTO
{
    public class TrainerWithPokemonDTO
    {
        public required int Id { get; set; }

        public required int BoxId { get; set; }

        [NotMapped]
        public List<Pokemon>? Pokemons { get; set; }
    }
}
