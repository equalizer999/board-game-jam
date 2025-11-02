using BoardGameCafe.Api.Data;
using BoardGameCafe.Api.Features.Orders;
using BoardGameCafe.Api.Features.Events;
using BoardGameCafe.Api.Features.Games;
using BoardGameCafe.Api.Features.Menu;
using BoardGameCafe.Api.Features.Reservations;
using BoardGameCafe.Api.Features.Customers;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext with SQLite
builder.Services.AddDbContext<BoardGameCafeDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost5173", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Add Health Checks
builder.Services.AddHealthChecks();

// Add services
builder.Services.AddScoped<OrderCalculationService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Board Game Café API", 
        Version = "v1",
        Description = "API for managing board game café reservations, games, and customers"
    });
    
    // Enable XML comments
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

// Seed database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<BoardGameCafeDbContext>();
        context.Database.Migrate(); // Apply pending migrations
        SeedData.Initialize(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

// Configure middleware
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Board Game Café API V1");
});

app.UseCors("AllowLocalhost5173");

// Health check endpoint
app.MapHealthChecks("/health");

// Placeholder minimal endpoint
app.MapGet("/api/v1/health", () => new { status = "ok", timestamp = DateTimeOffset.UtcNow })
    .WithName("GetHealthStatus")
    .WithTags("Health");

// Map feature endpoints
app.MapGamesEndpoints();
app.MapReservationsEndpoints();
app.MapOrdersEndpoints();
app.MapEventsEndpoints();
app.MapMenuEndpoints();
app.MapCustomersEndpoints();

app.Run();

// Make Program accessible for integration tests
public partial class Program { }
