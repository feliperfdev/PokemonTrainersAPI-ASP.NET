using DOTNETPokemonAPI.Models;

namespace DOTNETPokemonAPI.DTO
{
    public class TransferedPokemonDTO
    {
        public required int TrainerId { get; set; }
        public required int BoxId { get; set; }
        public required List<Pokemon> TransferedToBox {  get; set; }

        public required List<Pokemon> TransferedToParty { get; set; }
    }
}
