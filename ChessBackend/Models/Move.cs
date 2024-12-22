namespace ChessBackend.Models
{
    public class Move
    {
        // Unique identifier for the move
        public int Id { get; set; }

        // Foreign key to the associated game
           public int GameId { get; set; } 

        // The sequential order of the move within the game
        public int MoveOrder { get; set; }

        // Textual representation of the move (e.g., "e2 to e4")
        public required string MoveData { get; set; }

        // Optional: Tracking when the move was made
        public DateTime PlayedAt { get; set; } = DateTime.UtcNow;

        // Navigation property to the associated game
        public Game Game { get; set; } // The game that the move belongs to
    }
}
