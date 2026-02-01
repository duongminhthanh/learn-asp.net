using GameStore.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Data
{
    public class GameStoreContext(DbContextOptions<GameStoreContext> options) :
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

            modelBuilder.Entity<Game>().HasData(
                new { Id = 1, Name = "Minecraft", GenreId = 5, Price = 19.99m, ReleaseDate = new DateOnly(2011, 11, 18) },
                new { Id = 2, Name = "Street Fighter II", GenreId = 1, Price = 9.99m, ReleaseDate = new DateOnly(1991, 2, 1) },
                new { Id = 3, Name = "Final Fantasy XIV", GenreId = 2, Price = 59.99m, ReleaseDate = new DateOnly(2010, 9, 30) }
            );
        }
    }
}
