namespace DOTNETPokemonAPI.DTO
{
    public class TrainerWithPokemonDTO
    {
        public required int Id { get; set; }
        public required String Name { get; set; }

        public required List<int> PokemonIds { get; set; }

        public int? BoxPcId { get; set; }
    }
}
