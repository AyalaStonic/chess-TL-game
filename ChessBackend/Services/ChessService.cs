using ChessBackend.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace ChessBackend.Services
{
    public class ChessService : IChessService
    {
        private readonly IConfiguration _configuration;

        public ChessService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Get all games
        public IEnumerable<Game> GetAllGames()
        {
            var games = new List<Game>();
            using (var connection = new SqlConnection(_configuration.GetConnectionString("ChessDB")))
            {
                connection.Open();
                var command = new SqlCommand("SELECT * FROM Games", connection);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        games.Add(new Game
                        {
                            Id = (int)reader["Id"],
                            Name = (string)reader["Name"],
                            Status = (string)reader["Status"],
                            CreatedAt = (DateTime)reader["CreatedAt"],
                            EndedAt = reader["EndedAt"] as DateTime?
                        });
                    }
                }
            }
            return games;
        }

        // Get a specific game by its ID
        public Game? GetGameById(int id)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("ChessDB")))
            {
                connection.Open();
                var command = new SqlCommand("SELECT * FROM Games WHERE Id = @Id", connection);
                command.Parameters.AddWithValue("@Id", id);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Game
                        {
                            Id = (int)reader["Id"],
                            Name = (string)reader["Name"],
                            Status = (string)reader["Status"],
                            CreatedAt = (DateTime)reader["CreatedAt"],
                            EndedAt = reader["EndedAt"] as DateTime?
                        };
                    }
                }
            }
            return null;
        }

        // Add a new game
        public void AddGame(Game game)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("ChessDB")))
            {
                connection.Open();
                var command = new SqlCommand("INSERT INTO Games (Name, Status, CreatedAt) OUTPUT INSERTED.Id VALUES (@Name, @Status, @CreatedAt)", connection);
                command.Parameters.AddWithValue("@Name", game.Name);
                command.Parameters.AddWithValue("@Status", game.Status);
                command.Parameters.AddWithValue("@CreatedAt", game.CreatedAt);

                game.Id = (int)command.ExecuteScalar();
            }
        }

        // Add a move to a specific game by game ID
        public void AddMove(int gameId, string move)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("ChessDB")))
            {
                connection.Open();
                var command = new SqlCommand("INSERT INTO Moves (GameId, Move, MoveOrder) VALUES (@GameId, @Move, (SELECT COUNT(*) + 1 FROM Moves WHERE GameId = @GameId))", connection);
                command.Parameters.AddWithValue("@GameId", gameId);
                command.Parameters.AddWithValue("@Move", move);

                command.ExecuteNonQuery();
            }
        }

        // Get all moves for a specific game
        public List<Move> GetMovesForGame(int gameId)
        {
            var moves = new List<Move>();

            using (var connection = new SqlConnection(_configuration.GetConnectionString("ChessDB")))
            {
                connection.Open();
                var command = new SqlCommand("SELECT * FROM Moves WHERE GameId = @GameId ORDER BY MoveOrder", connection);
                command.Parameters.AddWithValue("@GameId", gameId);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        moves.Add(new Move
                        {
                            Id = (int)reader["Id"],
                            GameId = (int)reader["GameId"],
                            MoveText = (string)reader["Move"],
                            MoveOrder = (int)reader["MoveOrder"]
                        });
                    }
                }
            }

            return moves;
        }

        // Start a new game
        public Game StartNewGame()
        {
            var newGame = new Game
            {
                Name = $"Game {DateTime.UtcNow.Ticks}",
                Status = "In Progress",
                CreatedAt = DateTime.UtcNow
            };

            AddGame(newGame);
            return newGame;
        }

        // Reset a specific game by game ID
        public Game ResetGame(int gameId)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("ChessDB")))
            {
                connection.Open();

                // Delete moves
                var deleteCommand = new SqlCommand("DELETE FROM Moves WHERE GameId = @GameId", connection);
                deleteCommand.Parameters.AddWithValue("@GameId", gameId);
                deleteCommand.ExecuteNonQuery();

                // Reset the game's status
                var updateCommand = new SqlCommand("UPDATE Games SET Status = 'In Progress', EndedAt = NULL WHERE Id = @GameId", connection);
                updateCommand.Parameters.AddWithValue("@GameId", gameId);
                updateCommand.ExecuteNonQuery();
            }

            return GetGameById(gameId)!;
        }

        // Mark a game as completed
        public void CompleteGame(int gameId)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("ChessDB")))
            {
                connection.Open();
                var command = new SqlCommand("UPDATE Games SET Status = 'Completed', EndedAt = @EndedAt WHERE Id = @GameId", connection);
                command.Parameters.AddWithValue("@EndedAt", DateTime.UtcNow);
                command.Parameters.AddWithValue("@GameId", gameId);

                command.ExecuteNonQuery();
            }
        }

        // Save a game (Update the game state in the database)
        public void SaveGame(Game game)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("ChessDB")))
            {
                connection.Open();
                var command = new SqlCommand("UPDATE Games SET Name = @Name, Status = @Status, CreatedAt = @CreatedAt, EndedAt = @EndedAt WHERE Id = @Id", connection);
                command.Parameters.AddWithValue("@Name", game.Name);
                command.Parameters.AddWithValue("@Status", game.Status);
                command.Parameters.AddWithValue("@CreatedAt", game.CreatedAt);
                command.Parameters.AddWithValue("@EndedAt", game.EndedAt ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Id", game.Id);

                command.ExecuteNonQuery();
            }
        }
    }
}
