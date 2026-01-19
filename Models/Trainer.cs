using System.ComponentModel.DataAnnotations.Schema;

namespace DOTNETPokemonAPI.Models
{
    public class Trainer(String name)
    {
        public required int Id { get; set; }
        public required String Name { get; set; } = name;

        public required List<int> PokemonIds { get; set; }

        [NotMapped]
        public List<Pokemon> Pokemons { get; set; }

        [NotMapped]
        public List<Pokemon> PokemonsInBoxPc {  get; set; }

        public int? BoxPcId { get; set; }
    }
}
