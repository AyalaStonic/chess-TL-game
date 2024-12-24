using Microsoft.EntityFrameworkCore;

namespace ChessBackend.Models
{
    public class ChessDbContext : DbContext
    {
        // Constructor to accept DbContextOptions
        public ChessDbContext(DbContextOptions<ChessDbContext> options)
            : base(options)
        {
        }

        // DbSet properties for your entities
        public DbSet<User> Users { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<Move> Moves { get; set; }
        public DbSet<MoveData> MoveData { get; set; }

        // Override OnModelCreating to configure entity relationships
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure MoveData as an entity with a primary key
            modelBuilder.Entity<MoveData>()
                .HasKey(md => md.Id);

            // Configure Move entity
            modelBuilder.Entity<Move>()
                .HasKey(m => m.Id); // Primary key

            modelBuilder.Entity<Move>()
                .HasOne(m => m.Game) // One-to-Many relationship with Game
                .WithMany(g => g.Moves)
                .HasForeignKey(m => m.GameId);

            modelBuilder.Entity<Move>()
                .HasOne(m => m.MoveData) // One-to-One relationship with MoveData
                .WithOne()
                .HasForeignKey<Move>(m => m.MoveDataId);

            // Configure User entity
            modelBuilder.Entity<User>()
                .HasKey(u => u.Id); // Primary key

            modelBuilder.Entity<User>()
                .HasMany(u => u.Games) // One-to-Many relationship with Games
                .WithOne(g => g.User)
                .HasForeignKey(g => g.UserId);
        }
    }
}
