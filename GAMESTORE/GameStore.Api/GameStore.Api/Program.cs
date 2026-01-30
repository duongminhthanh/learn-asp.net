using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Define a connection string
var connString = builder.Configuration.GetConnectionString("GameStore");
builder.Services.AddSqlite<GameStoreContext>(connString);
// Registers GameStoreContext with a 'Scoped' lifetime in the Dependency Injection container.
// 'Scoped' means a new instance of GameStoreContext is created once per client request (HTTP request).
// It ensures that the same database connection is reused throughout a single request's lifecycle.
builder.Services.AddScoped<GameStoreContext>();
var app = builder.Build();

app.MapGameEndpoints();

app.MigrateDb();
app.Run();
