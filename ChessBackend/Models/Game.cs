using System.ComponentModel.DataAnnotations;

namespace ChessBackend.Models
{
    public class Game
    {
        public int Id { get; set; }
        
        // Add initialization or make nullable
        [Required]
        public string Moves { get; set; } = string.Empty;  // Initializes Moves with an empty string
    }
}
