namespace ChessBackend.Models
{
    public class User
    {
        // Unique identifier for the user
        public int Id { get; set; }


        // User's username
       public string Username { get; set; }// Add the Username property


        // List of games associated with the user (one-to-many relationship)
        public List<Game> Games { get; set; } = new List<Game>();
    }
}
