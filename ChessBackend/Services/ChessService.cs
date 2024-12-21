using ChessBackend.Models;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;  // Import async Task

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
        public async Task<IEnumerable<Game>> GetAllGames()
        {
            var games = new List<Game>();
            using (var connection = new SqlConnection(_configuration.GetConnectionString("ChessDB")))
            {
                await connection.OpenAsync();  // Asynchronous open
                var command = new SqlCommand("SELECT * FROM Games", connection);
                using (var reader = await command.ExecuteReaderAsync())  // Asynchronous read
                {
                    while (await reader.ReadAsync())
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
        public async Task<Game?> GetGameById(int id)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("ChessDB")))
            {
                await connection.OpenAsync();
                var command = new SqlCommand("SELECT * FROM Games WHERE Id = @Id", connection);
                command.Parameters.AddWithValue("@Id", id);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
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
        public async Task AddGame(Game game)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("ChessDB")))
            {
                await connection.OpenAsync();
                var command = new SqlCommand("INSERT INTO Games (Name, Status, CreatedAt) OUTPUT INSERTED.Id VALUES (@Name, @Status, @CreatedAt)", connection);
                command.Parameters.AddWithValue("@Name", game.Name);
                command.Parameters.AddWithValue("@Status", game.Status);
                command.Parameters.AddWithValue("@CreatedAt", game.CreatedAt);

                game.Id = (int)await command.ExecuteScalarAsync();  // Asynchronous query execution
            }
        }

        // Add a move to a specific game by game ID
        public async Task AddMove(int gameId, Move move)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("ChessDB")))
            {
                await connection.OpenAsync();
                var command = new SqlCommand("INSERT INTO Moves (GameId, Move, MoveOrder) VALUES (@GameId, @Move, (SELECT COUNT(*) + 1 FROM Moves WHERE GameId = @GameId))", connection);
                command.Parameters.AddWithValue("@GameId", gameId);
                command.Parameters.AddWithValue("@Move", move.MoveData);  // Assuming MoveData stores the move representation

                await command.ExecuteNonQueryAsync();  // Asynchronous query execution
            }
        }

        // Get all moves for a specific game
        public async Task<List<Move>> GetMovesForGame(int gameId)
        {
            var moves = new List<Move>();
            using (var connection = new SqlConnection(_configuration.GetConnectionString("ChessDB")))
            {
                await connection.OpenAsync();
                var command = new SqlCommand("SELECT * FROM Moves WHERE GameId = @GameId ORDER BY MoveOrder", connection);
                command.Parameters.AddWithValue("@GameId", gameId);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        moves.Add(new Move
                        {
                            Id = (int)reader["Id"],
                            GameId = (int)reader["GameId"],
                            MoveData = (string)reader["Move"],  // Adjusted to match your field
                            MoveOrder = (int)reader["MoveOrder"]
                        });
                    }
                }
            }

            return moves;
        }

        // Start a new game
        public async Task<Game> StartNewGame()
        {
            var newGame = new Game
            {
                Name = $"Game {DateTime.UtcNow.Ticks}",
                Status = "In Progress",
                CreatedAt = DateTime.UtcNow
            };

            await AddGame(newGame);
            return newGame;
        }

        // Reset a specific game by game ID
        public async Task<Game> ResetGame(int gameId)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("ChessDB")))
            {
                await connection.OpenAsync();

                // Delete moves
                var deleteCommand = new SqlCommand("DELETE FROM Moves WHERE GameId = @GameId", connection);
                deleteCommand.Parameters.AddWithValue("@GameId", gameId);
                await deleteCommand.ExecuteNonQueryAsync();

                // Reset the game's status
                var updateCommand = new SqlCommand("UPDATE Games SET Status = 'In Progress', EndedAt = NULL WHERE Id = @GameId", connection);
                updateCommand.Parameters.AddWithValue("@GameId", gameId);
                await updateCommand.ExecuteNonQueryAsync();
            }

            return await GetGameById(gameId)!;
        }

        // Mark a game as completed
        public async Task CompleteGame(int gameId)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("ChessDB")))
            {
                await connection.OpenAsync();
                var command = new SqlCommand("UPDATE Games SET Status = 'Completed', EndedAt = @EndedAt WHERE Id = @GameId", connection);
                command.Parameters.AddWithValue("@EndedAt", DateTime.UtcNow);
                command.Parameters.AddWithValue("@GameId", gameId);

                await command.ExecuteNonQueryAsync();
            }
        }

        // Save a game (Update the game state in the database)
        public async Task SaveGame(Game game)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("ChessDB")))
            {
                await connection.OpenAsync();
                var command = new SqlCommand("UPDATE Games SET Name = @Name, Status = @Status, CreatedAt = @CreatedAt, EndedAt = @EndedAt WHERE Id = @Id", connection);
                command.Parameters.AddWithValue("@Name", game.Name);
                command.Parameters.AddWithValue("@Status", game.Status);
                command.Parameters.AddWithValue("@CreatedAt", game.CreatedAt);
                command.Parameters.AddWithValue("@EndedAt", game.EndedAt ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Id", game.Id);

                await command.ExecuteNonQueryAsync();
            }
        }

        // Optional: Validate a move (you can implement this logic depending on the chess rules)
        public async Task<bool> ValidateMove(int gameId, Move move)
        {
            // Implement your move validation logic here (e.g., check if the move is legal)
            // For example:
            return true; // Assuming the move is valid (you can add your actual logic here)
        }
    }
}
