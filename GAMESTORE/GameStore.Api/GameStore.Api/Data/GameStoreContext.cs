using GameStore.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Data
{
    public class GameStoreContext(DbContextOptions<GameStoreContext> options):
        DbContext(options)
    {
        public DbSet<Game> Games => Set<Game>();
        public DbSet<Genre> Genres => Set<Genre>();

        //Seeding data
        // This method is called when the database model is being created
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure default seed data for the 'Genre' entity
            modelBuilder.Entity<Genre>().HasData(
                // Each 'new' object represents a row in the Genres table
                new { Id = 1, Name = "Fighting" },
                new { Id = 2, Name = "Roleplaying" },
                new { Id = 3, Name = "Sports" },
                new { Id = 4, Name = "Racing" },
                new { Id = 5, Name = "Kids and Family" }
            );
        }
    }
}
