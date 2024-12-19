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
        public List<string> Moves { get; set; } = new List<string>();

        // Optional: Tracking the date and time when the game was created
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;  // Default to current UTC time when the game is created
        
        // Optional: Tracking when the game ended
        public DateTime? EndedAt { get; set; }  // Nullable DateTime for end time (null if the game is ongoing)
    }
}
