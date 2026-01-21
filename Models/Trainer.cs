using System.ComponentModel.DataAnnotations.Schema;

namespace DOTNETPokemonAPI.Models
{
    public class Trainer(String name)
    {
        public required int Id { get; set; }
        public required String Name { get; set; } = name;

        public int? BoxPcId { get; set; }
    }
}
