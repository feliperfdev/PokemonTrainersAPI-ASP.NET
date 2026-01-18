namespace DOTNETPokemonAPI.Models
{
    public class BoxPC
    {
        public required int id {  get; set; }
        public List<Pokemon>? pokemons { get; set; } = new List<Pokemon>();
    }
}
