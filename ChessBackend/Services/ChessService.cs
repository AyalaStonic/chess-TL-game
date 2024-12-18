using ChessBackend.Models;
using System.Collections.Generic;
using System.Linq;

namespace ChessBackend.Services
{
    public class ChessService : IChessService
    {
        private readonly List<Game> _games;

        public ChessService()
        {
            // Initialize with some data or fetch from a database
            _games = new List<Game>
            {
                new Game { Id = 1, Name = "Game 1", Status = "In Progress", Moves = new List<string>() },
                new Game { Id = 2, Name = "Game 2", Status = "Completed", Moves = new List<string>() }
            };
        }

        // Get all games - returns a collection of games
        public IEnumerable<Game> GetAllGames()
        {
            return _games;
        }

        // Get a specific game by ID - returns nullable Game
        public Game? GetGameById(int id)
        {
            // If no game found, return null
            return _games.FirstOrDefault(g => g.Id == id);
        }

        // Add a new game to the list
        public void AddGame(Game game)
        {
            if (game != null)
            {
                _games.Add(game); // Add game logic, such as saving to a database or list
            }
        }

        // Add a move to a specific game - checks if game exists
        public void AddMove(int gameId, string move)
        {
            var game = GetGameById(gameId);

            if (game != null)
            {
                game.Moves.Add(move); // Add the move to the game's move list
                game.Status = "In Progress"; // Update game status if needed
            }
            else
            {
                // Handle the case where the game doesn't exist (optional)
                throw new KeyNotFoundException($"Game with ID {gameId} not found.");
            }
        }
    }
}
