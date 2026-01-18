using DOTNETPokemonAPI.Models;
using Microsoft.EntityFrameworkCore;



namespace DOTNETPokemonAPI.Database
{
    public class PokemonDb : DbContext
    {
        public PokemonDb(DbContextOptions<PokemonDb> options) : base(options)
        {
        }

        public DbSet<Trainer> Trainers => Set<Trainer>();
    }
}
