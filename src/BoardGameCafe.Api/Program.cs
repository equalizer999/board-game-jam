using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Board Game Café API", Version = "v1" });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Board Game Café API V1");
});

// Placeholder minimal endpoint
app.MapGet("/api/v1/health", () => new { status = "ok", timestamp = DateTimeOffset.UtcNow });

app.Run();
