namespace ChessBackend.Models
{
    public class Game
    {
        public int Id { get; set; }  // GameId equivalent

        // Use required to ensure Name is not null
        public required string Name { get; set; }

        // Status to track whether the game is ongoing or finished
        public required string Status { get; set; }

        // List of moves made during the game (each move as a string)
        public List<Move> Moves { get; set; } = new List<Move>();  // List of Move objects

        // Optional: Tracking the date and time when the game was created
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;  // Default to current UTC time when the game is created

        // Optional: Tracking when the game ended
        public DateTime? EndedAt { get; set; }  // Nullable DateTime for end time (null if the game is ongoing)

        // Additional Optional Property: Tracking the FEN (Forsyth-Edwards Notation) of the current game state
        public string? FEN { get; set; }  // Nullable string for FEN representation of the game state

        // Additional Optional Property: UserId if you wish to link a game to a user (for user management)
        public int? UserId { get; set; }  // Nullable UserId for optional user linkage
    }
}
