using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Entities;
using GameStore.Api.Mapping;
using Microsoft.EntityFrameworkCore;
namespace GameStore.Api.Endpoints
{
    public static class GamesEndpoints
    {
        const string GetGameEndpointName = "GetGame";

        public static RouteGroupBuilder MapGameEndpoints(this WebApplication app)
        {
            // Add group 
            var group = app.MapGroup("games").WithParameterValidation();
            // Get /games
            //Projection transform each game to game dto
            group.MapGet("/", (GameStoreContext dbContext) =>
            dbContext.Games
            // make sure each game has genre
            .Include(game => game.Genre)
            .Select(game => game.ToGameSummaryDto())
            // no tracking entities    
            .AsNoTracking());

            //Get /games/1
            //Inject db context
            group.MapGet("/{id}", (int id, GameStoreContext dbContext) =>
            {
                // Nullable game
                Game? game = dbContext.Games.Find(id);

                return game is null ?
                Results.NotFound() : Results.Ok(game.ToGameDetailsDto());
            })
            .WithName(GetGameEndpointName);

            // POST /games - Endpoint to create a new game record
            //Inject db context
            group.MapPost("/", (CreateGameDto newGame, GameStoreContext dbContext) =>
            {
                // 1. Map the incoming CreateGameDto (input) to a Game Entity (database model)
                Game game = newGame.ToEntity();

                // 2. Add the game entity to the Games DbSet for tracking
                dbContext.Games.Add(game);

                // 3. Persist changes to GameStore.db (generates and executes the SQL INSERT)
                dbContext.SaveChanges();

                // 4. Return 201 Created with the location of the new resource and the DTO body
                return Results.CreatedAtRoute(
                    GetGameEndpointName,
                    new { id = game.Id },
                    game.ToGameDetailsDto());
            });

            //Put /games/1
            //Inject db context
            group.MapPut("/{id}", (int id, UpdateGameDto updatedGame, GameStoreContext dbContext) =>
            {
                var existingGame = dbContext.Games.Find(id);

                if (existingGame is null)
                {
                    return Results.NotFound();
                }

                // 1. Cập nhật các giá trị cơ bản (Name, Price, ReleaseDate, GenreId)
                dbContext.Entry(existingGame)
                         .CurrentValues
                         .SetValues(updatedGame.ToEntity(id));

                // 2. QUAN TRỌNG: Ngắt kết nối với đối tượng Genre cũ trong bộ nhớ
                // Điều này buộc EF Core chỉ nhìn vào GenreId (con số 1) để lưu vào SQLite
                dbContext.Entry(existingGame)
                         .Reference(g => g.Genre)
                         .IsModified = false;
                existingGame.Genre = null;

                dbContext.SaveChanges();

                return Results.NoContent();
            });

            group.MapDelete("/{id}", (int id, GameStoreContext dbContext) =>
            {
                dbContext.Games.Where(game => game.Id == id)
                               .ExecuteDelete();

                return Results.NoContent();
            });
            
            return group;
        }
    }


}
