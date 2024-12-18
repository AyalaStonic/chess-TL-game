namespace ChessBackend.Models
{
    public class Game
    {
        public int Id { get; set; }  // GameId equivalent
        public required string Name { get; set; }
        public required string Status { get; set; }
        public List<string> Moves { get; set; } = new List<string>();  // Assuming Moves is a list of strings
    }
}
