using BoardGameRankings.DevTools;
using BoardGameRankings.Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

// Start BGG mock server in development
BggMockServer? bggMock = null;
if (builder.Environment.IsDevelopment())
{
    bggMock = BggMockServer.Start(9090);
    Log.Information("BGG mock server started at {Url}", bggMock.Url);
}

// Data storage path
var dataPath = Path.Combine(builder.Environment.ContentRootPath, "..", "..", "data");

// Register infrastructure + application services
builder.Services.AddInfrastructure(dataPath, builder.Configuration);

// Controllers
builder.Services.AddControllers();

// OpenAPI/Swagger
builder.Services.AddOpenApi();

// CORS for frontend dev server
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors();
app.MapControllers();

app.Lifetime.ApplicationStopping.Register(() => bggMock?.Dispose());

app.Run();
