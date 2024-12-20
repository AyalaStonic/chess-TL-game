using ChessBackend.Models;
using ChessBackend.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Register the services in the DI container
builder.Services.AddScoped<IChessService, ChessService>();

// Add controllers to the pipeline
builder.Services.AddControllers();

// Add CORS policy to allow requests from the Angular app on localhost:4200
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost", builder =>
    {
        builder.WithOrigins("http://localhost:4200") // Allow requests from Angular frontend
               .AllowAnyHeader()  // Allow any headers
               .AllowAnyMethod(); // Allow any HTTP methods
    });
});

// Add Swagger for API documentation (useful for testing and exploring the API)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline

if (app.Environment.IsDevelopment())
{
    // Enable Swagger in development mode for API documentation and testing
    app.UseSwagger();
    app.UseSwaggerUI(); // Automatically opens Swagger UI when you hit the API endpoint in a browser
}

// Use CORS policy for handling cross-origin requests (important for frontend-backend interaction)
app.UseCors("AllowLocalhost");

// Optional: Set Content Security Policy (CSP) header to restrict content sources
app.Use(async (context, next) =>
{
    context.Response.Headers["Content-Security-Policy"] = "script-src 'self' 'unsafe-eval'"; // Adjust based on your needs
    await next.Invoke();
});

// Use routing middleware to map incoming requests to appropriate endpoints
app.UseRouting();

// Map the controller routes to handle incoming API requests
app.MapControllers();

// Start the application
app.Run();
