using ChessBackend.Models;
using System.Collections.Generic;
using System.Linq;
using System;

namespace ChessBackend.Services
{
    public class ChessService : IChessService
    {
        private readonly List<Game> _games;

        public ChessService()
        {
            // Initializing the games list with some example games. 
            // You can replace this with actual database fetching logic when ready.
            _games = new List<Game>
            {
                new Game { Id = 1, Name = "Game 1", Status = "In Progress", Moves = new List<string>(), CreatedAt = DateTime.UtcNow },
                new Game { Id = 2, Name = "Game 2", Status = "Completed", Moves = new List<string>(), CreatedAt = DateTime.UtcNow }
            };
        }

        // Get all games
        public IEnumerable<Game> GetAllGames()
        {
            return _games;
        }

        // Get a specific game by its ID
        public Game? GetGameById(int id)
        {
            return _games.FirstOrDefault(g => g.Id == id);
        }

        // Add a new game
        public void AddGame(Game game)
        {
            if (game != null)
            {
                _games.Add(game); // Add the new game to the list (or a database in a real-world scenario)
            }
        }

        // Add a move to a specific game by game ID
        public void AddMove(int gameId, string move)
        {
            var game = GetGameById(gameId);
            if (game != null)
            {
                game.Moves.Add(move); // Add the move to the game's move list
                game.Status = "In Progress"; // Update the game status
            }
            else
            {
                throw new KeyNotFoundException($"Game with ID {gameId} not found."); // If the game doesn't exist
            }
        }

        // Start a new game
        public Game StartNewGame()
        {
            var newGame = new Game
            {
                Id = _games.Count + 1, // Auto-increment ID
                Name = $"Game {_games.Count + 1}",
                Status = "In Progress", // New game starts as "In Progress"
                Moves = new List<string>(), // Empty list of moves
                CreatedAt = DateTime.UtcNow // Set the creation time
            };

            _games.Add(newGame); // Add the new game to the list
            return newGame;
        }

        // Reset a specific game by game ID
        public Game ResetGame(int gameId)
        {
            var game = GetGameById(gameId);
            if (game != null)
            {
                game.Status = "In Progress"; // Reset game status to "In Progress"
                game.Moves.Clear(); // Clear the move history
            }
            else
            {
                throw new KeyNotFoundException($"Game with ID {gameId} not found."); // If the game doesn't exist
            }

            return game; // Return the reset game
        }

        // Mark a game as completed
        public void CompleteGame(int gameId)
        {
            var game = GetGameById(gameId);
            if (game != null)
            {
                game.Status = "Completed"; // Update status to "Completed"
                game.EndedAt = DateTime.UtcNow;  // Set the end time when the game is completed
            }
            else
            {
                throw new KeyNotFoundException($"Game with ID {gameId} not found."); // If the game doesn't exist
            }
        }

        // Implement the SaveGame method
        public void SaveGame(Game game)
        {
            // Check if the game exists by its ID
            var existingGame = _games.FirstOrDefault(g => g.Id == game.Id);
            if (existingGame != null)
            {
                // If the game exists, update its properties
                existingGame.Name = game.Name;
                existingGame.Status = game.Status;
                existingGame.Moves = game.Moves;
                existingGame.CreatedAt = game.CreatedAt;
                existingGame.EndedAt = game.EndedAt; // Update the end time if provided
            }
            else
            {
                // If the game doesn't exist, add it as a new game
                _games.Add(game);
            }
        }
    }
}
