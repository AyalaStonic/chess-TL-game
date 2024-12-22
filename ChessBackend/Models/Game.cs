namespace ChessBackend.Models
{
    public class Game
    {
        // Unique identifier for the game
        public int Id { get; set; }

        // Name of the game (e.g., "Game 1")
        public required string Name { get; set; }

        // Status to track the state of the game (e.g., "ongoing", "finished")
        public required string Status { get; set; }

        // List of moves made during the game (one-to-many relationship with Move)
        public List<Move> Moves { get; set; } = new List<Move>();

        // Date and time when the game was created
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Nullable: Date and time when the game ended
        public DateTime? EndedAt { get; set; }

        // The current game state in FEN format (can be used for replay)
        public string? FEN { get; set; }

        // Nullable: User ID if the game is linked to a user
        public int? UserId { get; set; }

        // Optional: Navigation property for user linking
        public User? User { get; set; } // User that the game is associated with (if applicable)
    }
}
