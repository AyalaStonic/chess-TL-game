namespace ChessBackend.Models
{
    // Class to represent the Move entity
    public class Move
    {
        // Unique identifier for the move
        public int Id { get; set; }

        // Foreign key to the associated game
        public int GameId { get; set; }

        // The sequential order of the move within the game
        public int MoveOrder { get; set; }

        // Move data: Object containing the 'from' and 'to' positions
         public MoveData MoveData { get; set; } 

        // Optional: Tracking when the move was made
        public DateTime PlayedAt { get; set; } = DateTime.UtcNow;

        // Navigation property to the associated game
        public Game Game { get; set; } // The game that the move belongs to
    }

    // Class to hold the actual move data, with 'from' and 'to' positions
    public class MoveData
    {
        // Starting position (e.g., "e2")
        public string From { get; set; }

        // Ending position (e.g., "e4")
        public string To { get; set; }
    }
}
