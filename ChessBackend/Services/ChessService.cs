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
            List<Game> games = new List<Game>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("SELECT GameId FROM Games", connection);

                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var gameId = reader.GetInt32(0);
                    var moves = GetMovesForGame(gameId);  // Ensure this method is public
                    games.Add(new Game { GameId = gameId, Moves = moves });
                }
            }

            return games;
        }

        // Get all moves for a specific game
        public List<string> GetMovesForGame(int gameId)  // Changed to public
        {
            List<string> moves = new List<string>();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("SELECT Move FROM Moves WHERE GameId = @GameId", connection);
                command.Parameters.AddWithValue("@GameId", gameId);

                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    moves.Add(reader.GetString(0));
                }
            }

            return moves;
        }

        // Method to create a new game and store moves in the database
        public void CreateGame(Game game)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand("INSERT INTO Games (CreatedAt) OUTPUT INSERTED.GameId VALUES (GETDATE())", connection);
                game.GameId = (int)command.ExecuteScalar();  // Get the generated GameId

                // Save the moves
                foreach (var move in game.Moves)
                {
                    SaveMove(game.GameId, move, connection);
                }
            }
        }

        // Method to save each move for a game
        private void SaveMove(int gameId, string move, SqlConnection connection)
        {
            var command = new SqlCommand("INSERT INTO Moves (GameId, Move) VALUES (@GameId, @Move)", connection);
            command.Parameters.AddWithValue("@GameId", gameId);
            command.Parameters.AddWithValue("@Move", move);
            command.ExecuteNonQuery();
        }
    }
}
