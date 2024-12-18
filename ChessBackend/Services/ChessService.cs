using ChessBackend.Models;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace ChessBackend.Services
{
    public class ChessService
    {
        private readonly string _connectionString;

        public ChessService(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Method to get all games
        public IEnumerable<Game> GetAllGames()
        {
            // For now, returning dummy data. Replace this with actual database calls.
            return new List<Game>
            {
                new Game { GameId = 1, Move = "e4", Moves = new List<string> { "e4", "e5" } },
                new Game { GameId = 2, Move = "d4", Moves = new List<string> { "d4", "d5" } }
            };
        }

        // Method to create a new game
        public void CreateGame(Game game)
        {
            // Logic to save the game in the database goes here. For now, it just generates a GameId.
            game.GameId = new Random().Next(1, 1000);  // Simulating DB-generated GameId

            // Normally, you'd save the game in your database here.
        }
    }
}
