using ChessBackend.Data;
using ChessBackend.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IChessService, ChessService>();
builder.Services.AddControllers();

// Add Swagger services
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ChessTLGameAPI", Version = "v1" });
});

var app = builder.Build();

// Enable Swagger UI
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Chess API v1");
    c.RoutePrefix = string.Empty;  // To serve Swagger at the root
});

// Force HTTPS redirection (if needed)
// app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();

app.Run();
