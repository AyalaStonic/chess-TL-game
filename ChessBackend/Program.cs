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

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost", builder =>
    {
        builder.WithOrigins("http://localhost:4200") // Allow requests from the Angular app
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

// Add Swagger (if you need to expose API documentation)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Set the Content Security Policy header (allows unsafe-eval temporarily for testing)
app.Use(async (context, next) =>
{
    // Use Append or indexer to set the header
    context.Response.Headers["Content-Security-Policy"] = "script-src 'self' 'unsafe-eval'";
    await next.Invoke();
});

// Use Swagger for API documentation (optional)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Use CORS policy
app.UseCors("AllowLocalhost");

// Use routing and controllers
app.UseRouting();
app.MapControllers();  // Maps the controller routes (like /api/chess/games)

app.Run();
