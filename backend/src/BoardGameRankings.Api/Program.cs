using BoardGameRankings.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Data storage path
var dataPath = Path.Combine(builder.Environment.ContentRootPath, "..", "..", "data");

// Register infrastructure + application services
builder.Services.AddInfrastructure(dataPath);

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

app.Run();
