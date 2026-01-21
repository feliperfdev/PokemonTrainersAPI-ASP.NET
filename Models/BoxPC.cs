using System.ComponentModel.DataAnnotations.Schema;

namespace DOTNETPokemonAPI.Models
{
    public class BoxPC
    {
        public required int Id {  get; set; }

        public Trainer? BoxTrainer { get; set; }
    }
}
