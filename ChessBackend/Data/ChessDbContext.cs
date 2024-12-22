using Microsoft.EntityFrameworkCore;

namespace ChessBackend.Models
{
    public class ChessDbContext : DbContext
    {
        public ChessDbContext(DbContextOptions<ChessDbContext> options)
            : base(options)
        {
        }

        // DbSets represent tables in the database
        public DbSet<Game> Games { get; set; }
        public DbSet<Move> Moves { get; set; }

        
        // DbSet for Users to enable user management
        public DbSet<User> Users { get; set; }

        // Configure the model relationships and constraints
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure the Game-Move relationship: One Game has many Moves
            modelBuilder.Entity<Game>()
                .HasMany(g => g.Moves)
                .WithOne(m => m.Game)  // Specify the navigation property on the Move side
                .HasForeignKey(m => m.GameId)
                .OnDelete(DeleteBehavior.Cascade); // When a game is deleted, its moves are also deleted

            // Configure the Game-User relationship: A Game may be linked to one User (nullable)
            modelBuilder.Entity<Game>()
                .HasOne(g => g.User)  // Specify the navigation property on the Game side
                .WithMany()  // Users can have multiple games
                .HasForeignKey(g => g.UserId)
                .OnDelete(DeleteBehavior.SetNull); // When a game is deleted, don't delete the user

            // Configure User entity constraints if necessary (e.g., setting properties as required)
            modelBuilder.Entity<User>()
                .HasKey(u => u.Id); // Ensure User ID is the primary key

            modelBuilder.Entity<User>()
                .Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(100); // Example of username property constraints

            // Additional configurations can be added here as needed (e.g., for validation, indexes, etc.)
        }
    }
}
