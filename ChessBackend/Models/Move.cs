namespace ChessBackend.Models
{
    public class Move
    {
        // The unique ID of the move
        public int Id { get; set; }

        // The ID of the game this move belongs to
        public int GameId { get; set; }

        // The move in standard algebraic notation (e.g., "e2 to e4")
        public required string MoveText { get; set; }

        // The order in which the move was made (e.g., 1 for the first move, 2 for the second, etc.)
        public int MoveOrder { get; set; }
    }
}
