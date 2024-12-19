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
                new Game { Id = 2, Name = "Game 2", Status = "Completed", Moves = new List<string>(), CreatedAt = DateTime.UtcNow, EndedAt = DateTime.UtcNow.AddHours(-1) }
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
                // Assign a new ID (This would be replaced by DB auto-generation logic in a real-world scenario)
                game.Id = _games.Count + 1;
                _games.Add(game); // Add the new game to the list (or a database in a real-world scenario)
            }
        }

        // Add a move to a specific game by game ID with validation
        public void AddMove(int gameId, string move)
        {
            var game = GetGameById(gameId);
            if (game == null)
            {
                throw new KeyNotFoundException($"Game with ID {gameId} not found.");
            }

            // Validate the move (This is just a placeholder; implement real chess move validation)
            if (!IsValidMove(game, move))
            {
                throw new ArgumentException("Invalid move.");
            }

            game.Moves.Add(move); // Add the move to the game's move list
            game.Status = "In Progress"; // Update the game status
        }

        // Validate a move (Placeholder for actual chess logic)
        private bool IsValidMove(Game game, string move)
        {
            // In a real-world scenario, you would validate the move using chess rules.
            return true; // Placeholder: Assume all moves are valid
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
            if (game == null)
            {
                throw new KeyNotFoundException($"Game with ID {gameId} not found.");
            }

            game.Status = "In Progress"; // Reset game status to "In Progress"
            game.Moves.Clear(); // Clear the move history

            return game; // Return the reset game
        }

        // Mark a game as completed
        public void CompleteGame(int gameId)
        {
            var game = GetGameById(gameId);
            if (game == null)
            {
                throw new KeyNotFoundException($"Game with ID {gameId} not found.");
            }

            game.Status = "Completed"; // Update status to "Completed"
            game.EndedAt = DateTime.UtcNow;  // Set the end time when the game is completed
        }

        // Save a game (Update the game state in the collection)
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
                game.Id = _games.Count + 1; // Generate a new ID for the game
                _games.Add(game); // Add it to the list
            }
        }
    }
}
