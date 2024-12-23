using ChessBackend.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace ChessBackend.Services
{
    public class ChessService : IChessService
    {
        private readonly IConfiguration _configuration;

        public ChessService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

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
                            UserId = reader["UserId"] as int?
                        });
                    }
                }
            }
            return games;
        }

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
                command.Parameters.AddWithValue("@UserId", game.UserId ?? (object)DBNull.Value);

                game.Id = (int)await command.ExecuteScalarAsync();
            }
        }

        public async Task AddMove(int gameId, Move move)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("ChessDB")))
            {
                await connection.OpenAsync();

                var insertMoveDataCommand = new SqlCommand(
                    "INSERT INTO MoveData ([From], [To]) OUTPUT INSERTED.Id VALUES (@From, @To)",
                    connection
                );
                insertMoveDataCommand.Parameters.AddWithValue("@From", move.MoveData.From);
                insertMoveDataCommand.Parameters.AddWithValue("@To", move.MoveData.To);

                int moveDataId = (int)await insertMoveDataCommand.ExecuteScalarAsync();

                var insertMoveCommand = new SqlCommand(
                    "INSERT INTO Moves (GameId, MoveDataId, MoveOrder, PlayedAt) " +
                    "VALUES (@GameId, @MoveDataId, (SELECT COUNT(*) + 1 FROM Moves WHERE GameId = @GameId), @PlayedAt)",
                    connection
                );
                insertMoveCommand.Parameters.AddWithValue("@GameId", gameId);
                insertMoveCommand.Parameters.AddWithValue("@MoveDataId", moveDataId);
                insertMoveCommand.Parameters.AddWithValue("@PlayedAt", move.PlayedAt);

                await insertMoveCommand.ExecuteNonQueryAsync();
            }
        }

        public async Task<Game> StartNewGame(int userId)
        {
            if (!await UserExists(userId))
            {
                throw new Exception("User not found");
            }

            var newGame = new Game
            {
                Name = "New Game",
                Status = "In Progress",
                CreatedAt = DateTime.UtcNow,
                UserId = userId
            };

            await AddGame(newGame);
            return newGame;
        }

        public async Task<Game> ResetGame(int gameId)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("ChessDB")))
            {
                await connection.OpenAsync();

                var deleteCommand = new SqlCommand("DELETE FROM Moves WHERE GameId = @GameId", connection);
                deleteCommand.Parameters.AddWithValue("@GameId", gameId);
                await deleteCommand.ExecuteNonQueryAsync();

                var updateCommand = new SqlCommand(
                    "UPDATE Games SET Status = 'In Progress', EndedAt = NULL WHERE Id = @GameId",
                    connection
                );
                updateCommand.Parameters.AddWithValue("@GameId", gameId);
                await updateCommand.ExecuteNonQueryAsync();
            }

            return await GetGameById(gameId) ?? throw new Exception("Game not found");
        }

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

        public async Task<bool> ValidateMove(int gameId, Move move)
        {
            // Placeholder for actual validation logic
            return true;
        }

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
                            MoveOrder = (int)reader["MoveOrder"],
                            PlayedAt = (DateTime)reader["PlayedAt"]
                        });
                    }
                }
            }
            return moves;
        }

        public async Task AddUserToGame(int gameId, int userId)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("ChessDB")))
            {
                await connection.OpenAsync();
                var command = new SqlCommand(
                    "INSERT INTO UsersGames (GameId, UserId) VALUES (@GameId, @UserId)",
                    connection
                );
                command.Parameters.AddWithValue("@GameId", gameId);
                command.Parameters.AddWithValue("@UserId", userId);

                await command.ExecuteNonQueryAsync();
            }
        }

        public async Task<IEnumerable<Game>> GetGamesByUserId(int userId)
        {
            var games = new List<Game>();
            using (var connection = new SqlConnection(_configuration.GetConnectionString("ChessDB")))
            {
                await connection.OpenAsync();
                var command = new SqlCommand(
                    "SELECT G.* FROM Games G JOIN UsersGames UG ON G.Id = UG.GameId WHERE UG.UserId = @UserId",
                    connection
                );
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
                            UserId = reader["UserId"] as int?
                        });
                    }
                }
            }
            return games;
        }


        public async Task<User> AddUser(User user)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("ChessDB")))
            {
                await connection.OpenAsync();
                var command = new SqlCommand(
                    "INSERT INTO Users (Username, CreatedAt) OUTPUT INSERTED.Id VALUES (@Username, @CreatedAt)",
                    connection
                );
                command.Parameters.AddWithValue("@Username", user.Username);
                command.Parameters.AddWithValue("@CreatedAt", DateTime.UtcNow);

                user.Id = (int)await command.ExecuteScalarAsync();
            }
            return user;
        }


public async Task<User?> GetUserByUsername(string username)
{
    using (var connection = new SqlConnection(_configuration.GetConnectionString("ChessDB")))
    {
        await connection.OpenAsync();
        var command = new SqlCommand("SELECT * FROM Users WHERE Username = @Username", connection);
        command.Parameters.AddWithValue("@Username", username);

        using (var reader = await command.ExecuteReaderAsync())
        {
            if (await reader.ReadAsync())
            {
                return new User
                {
                    Id = (int)reader["Id"],
                    Username = (string)reader["Username"],
                    CreatedAt = reader["CreatedAt"] != DBNull.Value 
                        ? (DateTime)reader["CreatedAt"] 
                        : DateTime.MinValue // Default value if null
                };
            }
        }
    }
    return null;
}




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
                            CreatedAt = (DateTime)reader["CreatedAt"]
                        };
                    }
                }
            }
            return null;
        }

        public async Task<bool> UserExists(int userId)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("ChessDB")))
            {
                await connection.OpenAsync();
                var command = new SqlCommand("SELECT COUNT(1) FROM Users WHERE Id = @UserId", connection);
                command.Parameters.AddWithValue("@UserId", userId);
                return (int)await command.ExecuteScalarAsync() > 0;
            }
        }

        public async Task<bool> UsernameExists(string username)
        {
            using (var connection = new SqlConnection(_configuration.GetConnectionString("ChessDB")))
            {
                await connection.OpenAsync();
                var command = new SqlCommand("SELECT COUNT(1) FROM Users WHERE Username = @Username", connection);
                command.Parameters.AddWithValue("@Username", username);
                return (int)await command.ExecuteScalarAsync() > 0;
            }
        }
    }
}
