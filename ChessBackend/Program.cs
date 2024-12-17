using ChessBackend.Services;

var builder = WebApplication.CreateBuilder(args);

// Register services
builder.Services.AddSingleton<ChessService>(provider =>
    new ChessService(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Add controllers
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseAuthorization();

app.MapControllers();

app.Run();
