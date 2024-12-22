namespace ChessBackend.Models
{
    public class User
    {
        // Unique identifier for the user
        public int Id { get; set; }

        // User's name (required)
        public required string Name { get; set; }

        // User's username
        public string Username { get; set; }  // Add the Username property

        // User's email address (required)
        public required string Email { get; set; }

        // List of games associated with the user (one-to-many relationship)
        public List<Game> Games { get; set; } = new List<Game>();
    }
}
