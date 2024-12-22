using ChessBackend.Models;
using ChessBackend.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Register services in the Dependency Injection container

// Register the ChessDbContext to interact with the database
builder.Services.AddDbContext<ChessDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ChessDB")));

// Register the Chess service to handle the game logic
builder.Services.AddScoped<IChessService, ChessService>();

// Add controllers to handle API endpoints
builder.Services.AddControllers();

// Configure CORS to allow requests from the Angular frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost", builder =>
    {
        builder.WithOrigins("http://localhost:4200") // Allow requests from the Angular app running on localhost:4200
               .AllowAnyHeader()                    // Allow any HTTP headers
               .AllowAnyMethod();                   // Allow any HTTP methods (GET, POST, PUT, DELETE)
    });
});

// Add Swagger for API documentation and testing
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    // Enable Swagger in development mode for API documentation
    app.UseSwagger();
    app.UseSwaggerUI(); // Automatically show Swagger UI for easier testing
}

// Use the CORS policy for handling cross-origin requests from the Angular frontend
app.UseCors("AllowLocalhost");

// Optional: Set Content Security Policy (CSP) header to enhance security
app.Use(async (context, next) =>
{
    context.Response.Headers["Content-Security-Policy"] = "script-src 'self' 'unsafe-eval';";
    await next.Invoke();
});

// Use routing middleware to map requests to appropriate controllers
app.UseRouting();

// Map controller routes for API endpoints
app.MapControllers();

// Run the application
app.Run();
