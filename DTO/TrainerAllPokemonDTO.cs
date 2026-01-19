using DOTNETPokemonAPI.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace DOTNETPokemonAPI.DTO
{
    public class TrainerAllPokemonDTO
    {
        public required int Id { get; set; }
        public required String Name { get; set; }

        [NotMapped]
        public List<Pokemon> AllPokemon {  get; set; } = new List<Pokemon>();
        
        public void AddPokemon(List<Pokemon> partyPokemon, List<Pokemon> boxPokemon)
        {
            AllPokemon = AllPokemon.Concat(boxPokemon).Concat(partyPokemon).ToList();
        }
    }
}
