namespace ChessBackend.Models
{
    public class Move
    {
        // The unique ID of the move  public int Id { get; set; }
        public int GameId { get; set; }
      public required string MoveData { get; set; }// Textual representation of the move (e.g., "e2 to e4")
        public int MoveOrder { get; set; }

        public int Id { get; set; }
    }
}
