namespace DOTNETPokemonAPI.Models
{
    public class Trainer(String name)
    {
        public required int Id { get; set; }
        public required String Name { get; set; } = name;

        public List<int> PokemonIds { get; private set; } = new List<int>();

        public int? BoxPcId { get; set; }

        public void AddPokemon(int pokemon)
        {

            if (PokemonIds.Count != 6) PokemonIds.Add(pokemon);
        }
    }
}
