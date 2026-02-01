using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Entities;
using GameStore.Api.Mapping;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
namespace GameStore.Api.Endpoints
{
    public static class GamesEndpoints
    {
        const string GetGameEndpointName = "GetGame";

        public static RouteGroupBuilder MapGameEndpoints(this WebApplication app)
        {
            // Add group 
            var group = app.MapGroup("games").WithParameterValidation();
            // GET /games
            // Retrieves all games with their associated genres asynchronously
            group.MapGet("/", async (GameStoreContext dbContext) =>
                await dbContext.Games
                    // Eagerly load the related Genre entity to avoid NullReferenceException
                    .Include(game => game.Genre)
                    // Project each Game entity into a GameSummaryDto for the response
                    .Select(game => game.ToGameSummaryDto())
                    // Improve performance by not tracking these entities in the DbContext
                    .AsNoTracking()
                    // Execute the query and return the list asynchronously
                    .ToListAsync());

            // GET /games/{id}
            // Fetches a single game by its unique identifier asynchronously
            group.MapGet("/{id}", async (int id, GameStoreContext dbContext) =>
            {
                // Find the game by ID using the asynchronous FindAsync method
                // Returns null if no record matches the given ID
                Game? game = await dbContext.Games.FindAsync(id);

                // Return 404 Not Found if the game is null; 
                // otherwise, map the entity to a DTO and return 200 OK
                return game is null ?
                    Results.NotFound() : Results.Ok(game.ToGameDetailsDto());
            })
            .WithName(GetGameEndpointName); // Assigned a name for route referencing (e.g., in CreatedAtRoute)

            // POST /games
            // Creates a new game record in the database asynchronously
            group.MapPost("/", async (CreateGameDto newGame, GameStoreContext dbContext) =>
            {
                // 1. Map the input DTO to a Game Entity using an extension method
                Game game = newGame.ToEntity();

                // 2. Inform the context to track the new entity for insertion
                dbContext.Games.Add(game);

                // 3. Commit the transaction to the database and generate the new ID
                await dbContext.SaveChangesAsync();

                // 4. Map the persisted entity to a Details DTO and return 201 Created
                // Providing the route name and ID allows clients to locate the new resource
                return Results.CreatedAtRoute(
                    GetGameEndpointName,
                    new { id = game.Id },
                    game.ToGameDetailsDto());
            });

            // PUT /games/{id}
            // Updates an existing game record asynchronously
            group.MapPut("/{id}", async (int id, UpdateGameDto updatedGame, GameStoreContext dbContext) =>
            {
                // Fetch the existing game from the database
                var existingGame = await dbContext.Games.FindAsync(id);

                // Return 404 if the game doesn't exist
                if (existingGame is null)
                {
                    return Results.NotFound();
                }

                // 1. Update primitive properties (Name, Price, etc.) using SetValues
                dbContext.Entry(existingGame)
                         .CurrentValues
                         .SetValues(updatedGame.ToEntity(id));

                // 2. CRITICAL: Detach the Genre navigation property to prevent SQLite FK conflicts
                // This forces EF Core to only update the GenreId column in the database
                dbContext.Entry(existingGame)
                         .Reference(g => g.Genre)
                         .IsModified = false;

                existingGame.Genre = null;

                // 3. Persist changes to the database asynchronously
                await dbContext.SaveChangesAsync();

                // 204 NoContent is the standard response for a successful update
                return Results.NoContent();
            });

            group.MapDelete("/{id}", async (int id, GameStoreContext dbContext) =>
            {
                await dbContext.Games.Where(game => game.Id == id)
                               .ExecuteDeleteAsync();

                return Results.NoContent();
            });

            return group;
        }
    }


}
