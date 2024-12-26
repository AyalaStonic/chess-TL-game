namespace ChessBackend.Models
{
    
    public class MoveData
    {

        public int Id { get; set; } // Ensure this property exists
       public int GameId { get; set; } // Primary key
       public string From { get; set; } // Starting square
       public string To { get; set; }  // Destination square
    }
}
