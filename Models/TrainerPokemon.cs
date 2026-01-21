using System.ComponentModel.DataAnnotations.Schema;

namespace DOTNETPokemonAPI.Models
{
    public class TrainerPokemon
    {
        public required int Id { get; set; }
        public required int PokemonId { get; set; }
        public required int TrainerId { get; set; }
        public required DateTime CapturedAt { get; set; }
        public required String Location { get; set; }

        [NotMapped]
        public String Name { get; set; }
    }
}
