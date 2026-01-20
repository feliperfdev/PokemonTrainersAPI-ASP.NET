using System.Text.Json.Serialization;

namespace DOTNETPokemonAPI.DTO
{
    public class PokemonFromBoxToMoveDTO
    {
        public required List<int> PokemonToMoveIds { get; set; }
        public required int TrainerId { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public required MoveTypeEnum Type { get; set; }
    }
}
