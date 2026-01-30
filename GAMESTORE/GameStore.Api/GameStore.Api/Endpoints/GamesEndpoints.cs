using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Entities;
using GameStore.Api.Mapping;
namespace GameStore.Api.Endpoints
{
    public static class GamesEndpoints
    {
        const string GetGameEndpointName = "GetGame";
        private static readonly List<GameDto> games = [
        new (
            1,
            "Street Fighter II",
            "Fighting",
            19.99M,
            new DateOnly(1992, 7, 15)
        ),
        new (
            2,
            "Final Fantasy XIV",
            "Roleplaying",
            59.99M,
            new DateOnly(2010, 9, 30)
        ),
        new (
            3,
            "FIFA 23",
            "Sports",
            69.99M,
            new DateOnly(2022, 9, 27))
        ];

        public static RouteGroupBuilder MapGameEndpoints(this WebApplication app)
        {
            // Add group 
            var group = app.MapGroup("games").WithParameterValidation();
            // Get /games
            group.MapGet("/", () => games);

            //Get /games/1
            group.MapGet("/{id}", (int id) =>
            {
                // Nullable game
                GameDto? game = games.Find(game => game.Id == id);

                return game is null ? Results.NotFound() : Results.Ok(game);
            })
            .WithName(GetGameEndpointName);

            // POST /games - Endpoint to create a new game record
            group.MapPost("/", (CreateGameDto newGame, GameStoreContext dbContext) =>
            {
                // 1. Map the incoming CreateGameDto (input) to a Game Entity (database model)
                Game game = newGame.ToEntity();
                game.Genre = dbContext.Genres.Find(newGame.GenreId);

                // 2. Add the game entity to the Games DbSet for tracking
                dbContext.Games.Add(game);

                // 3. Persist changes to GameStore.db (generates and executes the SQL INSERT)
                dbContext.SaveChanges();

                // 4. Return 201 Created with the location of the new resource and the DTO body
                return Results.CreatedAtRoute(
                    GetGameEndpointName,
                    new { id = game.Id },
                    game.ToDto());
            });

            //Put /games/1
            group.MapPut("/{id}", (int id, UpdateGameDto updatedGame) =>
            {
                // Find index of game base on id
                var index = games.FindIndex(game => game.Id == id);

                // index not found
                if (index == -1)
                {
                    return Results.NotFound();
                }

                games[index] = new GameDto(
                    id,
                    updatedGame.Name,
                    updatedGame.Genre,
                    updatedGame.Price,
                    updatedGame.ReleaseDate
                );

                return Results.NoContent();
            });

            group.MapDelete("/{id}", (int id) =>
            {
                games.RemoveAll(game => game.Id == id);

                return Results.NoContent();
            });

            return group;
        }
    }


}
