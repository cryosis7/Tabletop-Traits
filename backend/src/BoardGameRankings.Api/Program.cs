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
var configuredDataPath = builder.Configuration["DataPath"];
var dataPath = string.IsNullOrWhiteSpace(configuredDataPath)
    ? Path.Combine(builder.Environment.ContentRootPath, "..", "..", "data")
    : Path.GetFullPath(configuredDataPath);

// Register infrastructure + application services
builder.Services.AddInfrastructure(dataPath, builder.Configuration);

// Controllers
builder.Services.AddControllers();

// OpenAPI
builder.Services.AddOpenApi("v1", options =>
{
    options.AddDocumentTransformer((document, context, cancellationToken) =>
    {
        document.Info.Title = "Board Game Rankings API";
        document.Info.Version = "v1";
        document.Info.Description = "Synchronizes BoardGameGeek collections, exposes stored rated games, and returns mechanism-based preference analysis for a user.";

        return Task.CompletedTask;
    });
});

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

    app.UseSwaggerUI(options =>
    {
        options.RoutePrefix = "swagger";
        options.DocumentTitle = "Board Game Rankings API Docs";
        options.SwaggerEndpoint("/openapi/v1.json", "Board Game Rankings API v1");
    });
}

app.UseCors();
app.MapControllers();

app.Lifetime.ApplicationStopping.Register(() => bggMock?.Dispose());

app.Run();
