namespace ChessBackend.Models
{
    public class Move
    {
        public int Id { get; set; } // Primary key
        public int GameId { get; set; } // Foreign key to Game
        public int MoveOrder { get; set; } // Order of the move
        public int MoveDataId { get; set; } // Foreign key to MoveData

        public DateTime PlayedAt { get; set; } = DateTime.UtcNow; // Timestamp of the move

        // Navigation properties
        public Game Game { get; set; } // Navigation to Game
        public MoveData MoveData { get; set; } // Navigation to MoveData
    }

}