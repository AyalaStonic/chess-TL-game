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

// Add Swagger for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline

if (app.Environment.IsDevelopment())
{
    // Use Swagger in development mode
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Use CORS policy
app.UseCors("AllowLocalhost");

// Set the Content Security Policy header (optional, specific to your needs)
app.Use(async (context, next) =>
{
    context.Response.Headers["Content-Security-Policy"] = "script-src 'self' 'unsafe-eval'";
    await next.Invoke();
});

// Use routing middleware
app.UseRouting();

// Map controller routes
app.MapControllers();

app.Run();
