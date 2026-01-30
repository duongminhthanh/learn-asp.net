using GameStore.Api.Dtos;
using GameStore.Api.Entities;

namespace GameStore.Api.Mapping
{
    public static class GameMapping
    {
        public static Game ToEntity(this CreateGameDto game)
        {
            return new Game()
            {
                Name = game.Name,
                GenreId = game.GenreId,
                Price = game.Price,
                ReleaseDate = game.ReleaseDate

            };

        }

        public static GameDto ToDto(this Game game)
        {
            return new
            (
                 game.Id,
                 game.Name,
                 game.Genre!.Name, // Use '!' because we expect the genre to exist after seeding
                 game.Price,
                 game.ReleaseDate
            );
        }
    }
}
