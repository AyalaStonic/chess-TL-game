using ChessBackend.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using ChessDotNet;
using ChessBackendMove = ChessBackend.Models.Move; // Alias for the backend Move class
using ChessDotNetMove = ChessDotNet.Move; // Alias for ChessDotNet Move class

namespace ChessBackend.Services
{
    public class ChessService : IChessService
    {
        private readonly ChessDbContext _context; // Inject ChessDbContext
        private readonly IConfiguration _configuration;

        // Constructor injecting ChessDbContext and IConfiguration
        public ChessService(ChessDbContext context, IConfiguration configuration)
        {
            _context = context;
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


      public async Task AddMove(int gameId, ChessBackendMove move)
{
    using (var connection = new SqlConnection(_configuration.GetConnectionString("ChessDB")))
    {
        await connection.OpenAsync();

        // Insert MoveData into the database
        var insertMoveDataCommand = new SqlCommand(
            "INSERT INTO MoveData ([From], [To]) OUTPUT INSERTED.Id VALUES (@From, @To)",
            connection
        );
        insertMoveDataCommand.Parameters.AddWithValue("@From", move.MoveData.From);
        insertMoveDataCommand.Parameters.AddWithValue("@To", move.MoveData.To);

        int moveDataId = (int)await insertMoveDataCommand.ExecuteScalarAsync();

        // Insert the Move entry, linked to MoveData
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
    // Ensure the user exists before starting the game
    if (!await UserExists(userId))
    {
        throw new Exception("User not found");
    }

    // Set the initial Fen string to represent the standard starting chessboard position
    string initialFen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";  // Standard FEN for the starting position

    // Create a new game object
    var newGame = new Game
    {
        Name = "New Game",
        Status = "In Progress",
        CreatedAt = DateTime.UtcNow,
        UserId = userId,
        Fen = initialFen
    };

    // Add the newly created game to the database
    await AddGame(newGame);

    return newGame;
}

private bool IsValidFen(string fen)
{
    var parts = fen.Split(' ');
    if (parts.Length != 6)
    {
        return false; // There should be exactly 6 parts (board state, turn, castling, en passant, half-move, full-move)
    }

    var board = parts[0];
    var ranks = board.Split('/');
    if (ranks.Length != 8) return false;  // The board must have 8 ranks

    foreach (var rank in ranks)
    {
        int rankLength = 0;
        foreach (char c in rank)
        {
            if (char.IsDigit(c))
            {
                rankLength += (int)char.GetNumericValue(c);  // Add the number of empty squares
            }
            else if ("rnbqkRNBQK".Contains(c))  // Valid piece characters
            {
                rankLength++;
            }
            else
            {
                return false;  // Invalid character found
            }
        }

        if (rankLength != 8) return false;  // Each rank must have exactly 8 squares
    }

    // Check for valid turn and other parts (w or b for turn, KQkq for castling, etc.)
    if (!new[] { "w", "b" }.Contains(parts[1])) return false;  // Turn to move (white or black)
    if (!IsValidCastling(parts[2])) return false;  // Castling rights (KQkq)
    if (!string.IsNullOrEmpty(parts[3]) && parts[3] != "-") return false;  // En passant target (should be '-' or valid square)
    if (int.TryParse(parts[4], out int halfMove) && halfMove < 0) return false;  // Half-move clock
    if (int.TryParse(parts[5], out int fullMove) && fullMove < 1) return false;  // Full-move number

    return true;
}

private bool IsValidCastling(string castling)
{
    foreach (var c in castling)
    {
        if (!"KQkq".Contains(c))
        {
            return false;  // Invalid castling rights
        }
    }
    return true;
}


   public async Task AddGame(Game game)
{
    using (var connection = new SqlConnection(_configuration.GetConnectionString("ChessDB")))
    {
        await connection.OpenAsync();

        // Adjust the query to include the Fen column
        var command = new SqlCommand(
            "INSERT INTO Games (Name, Status, CreatedAt, UserId, Fen) OUTPUT INSERTED.Id VALUES (@Name, @Status, @CreatedAt, @UserId, @Fen)",
            connection
        );

        command.Parameters.AddWithValue("@Name", game.Name);
        command.Parameters.AddWithValue("@Status", game.Status);
        command.Parameters.AddWithValue("@CreatedAt", game.CreatedAt);
        command.Parameters.AddWithValue("@UserId", game.UserId ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@Fen", game.Fen);  // Add Fen to the insert statement

        // Execute the command and retrieve the inserted game Id
        game.Id = (int)await command.ExecuteScalarAsync();
    }
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

         // Updated ValidateMove method
        public async Task<bool> ValidateMove(int gameId, ChessBackendMove move)
        {
            // Fetch the game from the database using _context
            var game = await _context.Games.FindAsync(gameId);
            if (game == null)
            {
                return false; // Game not found
            }

            // Load the current game state using ChessDotNet.ChessGame
            var chessGame = new ChessDotNet.ChessGame(game.Fen);

            // Convert 'From' and 'To' to ChessDotNet.Position
            var fromPosition = new ChessDotNet.Position(move.MoveData.From);
            var toPosition = new ChessDotNet.Position(move.MoveData.To);

            // Create the move using ChessDotNet.Move
            var chessMove = new ChessDotNet.Move(fromPosition, toPosition, chessGame.WhoseTurn);

            // Validate the move using ChessDotNet
            ChessDotNet.MoveType moveResult = chessGame.MakeMove(chessMove, true); // 'true' for alreadyValidated flag
            
            // Return whether the move is valid or not
            return moveResult != ChessDotNet.MoveType.Invalid;
        }

  public async Task<List<ChessBackendMove>> GetMovesForGame(int gameId)
{
    var moves = new List<ChessBackendMove>(); // Use the alias for backend Move class
    using (var connection = new SqlConnection(_configuration.GetConnectionString("ChessDB")))
    {
        await connection.OpenAsync();
        var command = new SqlCommand("SELECT * FROM Moves WHERE GameId = @GameId ORDER BY MoveOrder", connection);
        command.Parameters.AddWithValue("@GameId", gameId);

        using (var reader = await command.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                moves.Add(new ChessBackendMove
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




public bool ApplyMove(ChessBackend.Models.Game game, ChessDotNet.Move chessMove)
{
    // Initialize the ChessDotNet game with the current game state (FEN string)
    var chessGame = new ChessDotNet.ChessGame(game.Fen);  // Use ChessDotNet.ChessGame instead of ChessDotNet.Game

    // Attempt to make the move
    ChessDotNet.MoveType moveResult = chessGame.MakeMove(chessMove, true);  // 'true' for already validated

    // If the move was not made successfully, return false
    if (moveResult == ChessDotNet.MoveType.Invalid)
    {
        return false; // Invalid move, return false
    }

    // If valid, update the FEN string with the new game state
    game.Fen = chessGame.GetFen();  // Get the updated FEN string

    return true; // Move applied successfully
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
