using ChessBackend.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;

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
                await connection.OpenAsync();
                var command = new SqlCommand("SELECT * FROM Games", connection);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        games.Add(new Game
                        {
                            Id = (int)reader["Id"],
                            Name = (string)reader["Name"],
                            Status = (string)reader["Status"],
                            CreatedAt = (DateTime)reader["CreatedAt"],
                            EndedAt = reader["EndedAt"] as DateTime?,
                            UserId = reader["UserId"] as int? // Link to the user who created the game
                        });
                    }
                }
            }
            return games;
        }

        // Get a specific game by ID
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
                            EndedAt = reader["EndedAt"] as DateTime?,
                            UserId = reader["UserId"] as int?
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
                var command = new SqlCommand(
                    "INSERT INTO Games (Name, Status, CreatedAt, UserId) OUTPUT INSERTED.Id VALUES (@Name, @Status, @CreatedAt, @UserId)",
                    connection
                );
                command.Parameters.AddWithValue("@Name", game.Name);
                command.Parameters.AddWithValue("@Status", game.Status);
                command.Parameters.AddWithValue("@CreatedAt", game.CreatedAt);
                command.Parameters.AddWithValue("@UserId", game.UserId ?? (object)DBNull.Value); // Handle nullable user ID

                game.Id = (int)await command.ExecuteScalarAsync();
            }
        }

        // Add a move to a specific game
public async Task AddMove(int gameId, Move move)
{
    using (var connection = new SqlConnection(_configuration.GetConnectionString("ChessDB")))
    {
        await connection.OpenAsync();

        // Serialize MoveData object to a JSON string (assuming MoveData is a class)
        var moveDataJson = JsonSerializer.Serialize(move.MoveData); // Serialize MoveData to JSON string

        var command = new SqlCommand(
            "INSERT INTO Moves (GameId, Move, MoveOrder) VALUES (@GameId, @Move, (SELECT COUNT(*) + 1 FROM Moves WHERE GameId = @GameId))",
            connection
        );

        // Add parameters to the SQL command
        command.Parameters.AddWithValue("@GameId", gameId);
        command.Parameters.AddWithValue("@Move", moveDataJson);  // Store the serialized JSON of MoveData

        await command.ExecuteNonQueryAsync();
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
                // Deserialize the MoveData (JSON string) back into a MoveData object
                var moveDataJson = (string)reader["Move"];
                var moveData = JsonSerializer.Deserialize<MoveData>(moveDataJson);  // Deserialize the JSON string back to MoveData

                moves.Add(new Move
                {
                    Id = (int)reader["Id"],
                    GameId = (int)reader["GameId"],
                    MoveData = moveData,  // Set MoveData (deserialized object)
                    MoveOrder = (int)reader["MoveOrder"]
                });
            }
        }
    }
    return moves;
}


        // Start a new game
        public async Task<Game> StartNewGame(int userId)
        {
            var newGame = new Game
            {
                Name = $"Game {DateTime.UtcNow.Ticks}",
                Status = "In Progress",
                CreatedAt = DateTime.UtcNow,
                UserId = userId // Link the game to the user
            };

            await AddGame(newGame);
            return newGame;
        }

        // Reset a specific game by ID
        public async Task<Game> ResetGame(int gameId)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("ChessDB")))
            {
                await connection.OpenAsync();

                // Delete moves
                var deleteCommand = new SqlCommand("DELETE FROM Moves WHERE GameId = @GameId", connection);
                deleteCommand.Parameters.AddWithValue("@GameId", gameId);
                await deleteCommand.ExecuteNonQueryAsync();

                // Reset game's status
                var updateCommand = new SqlCommand(
                    "UPDATE Games SET Status = 'In Progress', EndedAt = NULL WHERE Id = @GameId",
                    connection
                );
                updateCommand.Parameters.AddWithValue("@GameId", gameId);
                await updateCommand.ExecuteNonQueryAsync();
            }

            return await GetGameById(gameId) ?? throw new Exception("Game not found");
        }

        // Mark a game as completed
        public async Task CompleteGame(int gameId)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("ChessDB")))
            {
                await connection.OpenAsync();
                var command = new SqlCommand(
                    "UPDATE Games SET Status = 'Completed', EndedAt = @EndedAt WHERE Id = @GameId",
                    connection
                );
                command.Parameters.AddWithValue("@EndedAt", DateTime.UtcNow);
                command.Parameters.AddWithValue("@GameId", gameId);

                await command.ExecuteNonQueryAsync();
            }
        }

        // Save a game
        public async Task SaveGame(Game game)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("ChessDB")))
            {
                await connection.OpenAsync();
                var command = new SqlCommand(
                    "UPDATE Games SET Name = @Name, Status = @Status, CreatedAt = @CreatedAt, EndedAt = @EndedAt WHERE Id = @Id",
                    connection
                );
                command.Parameters.AddWithValue("@Name", game.Name);
                command.Parameters.AddWithValue("@Status", game.Status);
                command.Parameters.AddWithValue("@CreatedAt", game.CreatedAt);
                command.Parameters.AddWithValue("@EndedAt", game.EndedAt ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Id", game.Id);

                await command.ExecuteNonQueryAsync();
            }
        }

        // Validate a move (custom logic if needed)
        public async Task<bool> ValidateMove(int gameId, Move move)
        {
            // Placeholder for actual move validation logic
            return true;
        }

        // Add a user to a game (for linking games to users)
        public async Task AddUserToGame(int gameId, int userId)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("ChessDB")))
            {
                await connection.OpenAsync();
                var command = new SqlCommand(
                    "INSERT INTO UsersG (GameId, UserId) VALUES (@GameId, @UserId)",
                    connection
                );
                command.Parameters.AddWithValue("@GameId", gameId);
                command.Parameters.AddWithValue("@UserId", userId);

                await command.ExecuteNonQueryAsync();
            }
        }

        // Get games by user ID
public async Task<IEnumerable<Game>> GetGamesByUserId(int userId)
{
    var games = new List<Game>();
    using (var connection = new SqlConnection(_configuration.GetConnectionString("ChessDB")))
    {
        await connection.OpenAsync();
        
        // Fix the typo from 'U.Id' to 'G.Id' and ensure proper table and column references
        var command = new SqlCommand(
            "SELECT G.* FROM Games G " + 
            "JOIN UsersGames UG ON G.Id = UG.GameId " + // Corrected the JOIN condition
            "WHERE UG.UserId = @UserId", 
            connection
        );

        // Add the @UserId parameter
        command.Parameters.AddWithValue("@UserId", userId);

        using (var reader = await command.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                games.Add(new Game
                {
                    Id = (int)reader["Id"],
                    Name = reader["Name"] as string,
                    Status = reader["Status"] as string,
                    CreatedAt = (DateTime)reader["CreatedAt"],
                    EndedAt = reader["EndedAt"] as DateTime?,
                    UserId = reader["UserId"] as int? // This may not be needed if UserId is not in the Games table
                });
            }
        }
    }
    return games;
}

       // Add a user to the database
public async Task<User> AddUser(User user)
{
    if (string.IsNullOrWhiteSpace(user.Username)) // Only check if Username is provided
    {
        throw new ArgumentException("Username must be provided.");
    }

    using (var connection = new SqlConnection(_configuration.GetConnectionString("ChessDB")))
    {
        await connection.OpenAsync();
        var command = new SqlCommand(
            "INSERT INTO Users (Username) OUTPUT INSERTED.Id VALUES (@Username)",
            connection
        );
        command.Parameters.AddWithValue("@Username", user.Username);

        // Insert the user and get the generated Id.
        user.Id = (int)await command.ExecuteScalarAsync();
    }

    return user; // Return the created user with the generated Id.
}


        // Get a user by ID
        public async Task<User?> GetUserById(int userId)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("ChessDB")))
            {
                await connection.OpenAsync();
                var command = new SqlCommand("SELECT * FROM Users WHERE Id = @UserId", connection);
                command.Parameters.AddWithValue("@UserId", userId);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new User
                        {
                            Id = (int)reader["Id"],
                            Username = (string)reader["Username"],
                        };
                    }
                }
            }
            return null;
        }
    }
}
