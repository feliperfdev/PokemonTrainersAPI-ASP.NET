using DOTNETPokemonAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace DOTNETPokemonAPI.Database
{
    public class PokemonDb : DbContext
    {

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }

        public PokemonDb(DbContextOptions<PokemonDb> options) : base(options)
        {
        }

        public DbSet<Trainer> Trainers => Set<Trainer>();
        public DbSet<Pokemon> Pokemons => Set<Pokemon>();

        public DbSet<BoxPC> BoxPCs => Set<BoxPC>();
    }
}
