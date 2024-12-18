using ChessBackend.Models;
using ChessBackend.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Register the services
builder.Services.AddScoped<IChessService, ChessService>();

// Add controllers
builder.Services.AddControllers();

// Add Swagger (if you need to expose API documentation)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Use Swagger for API documentation (optional)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Use routing and controllers
app.UseRouting();

app.MapControllers();  // Maps the controller routes (like /api/chess/games)

app.Run();
