using System.ComponentModel.DataAnnotations.Schema;

namespace DOTNETPokemonAPI.Models
{
    public class BoxPC
    {
        public required int Id {  get; set; }

        public required List<int> PokemonIds { get; set; }

        [NotMapped]
        public List<Pokemon>? Pokemons { get; set; }

        public Trainer? BoxTrainer { get; set; }
    }
}
