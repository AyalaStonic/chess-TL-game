namespace ChessBackend.Models
{
    public class Game
    {
        public int GameId { get; set; }
        public string? Move { get; set; }  // Make Move nullable to avoid the warning
        public List<string> Moves { get; set; } = new List<string>();  // Initialize Moves
    }
}
