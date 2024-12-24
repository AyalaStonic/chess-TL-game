using ChessBackend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChessBackend.Services
{
    public interface IChessService
    {
        // Returns a list of all games
        Task<IEnumerable<Game>> GetAllGames();

        // Returns a game by its ID, nullable because it can return null if the game is not found
        Task<Game?> GetGameById(int id);

        // Adds a new game to the collection
        Task AddGame(Game game);

        // Adds a move to an existing game
        Task AddMove(int gameId, Move move);

        // Start a new game with a user ID (passing user ID to create a new game)
        Task<Game> StartNewGame(int userId);

        // Resets the current game
        Task<Game> ResetGame(int gameId);

        // Marks a game as completed (updates status and sets EndedAt)
        Task CompleteGame(int gameId);

        // Save the game state (update game in the database)
        Task SaveGame(Game game);

        // Validate a move before applying it
        Task<bool> ValidateMove(int gameId, Move move);

        // Returns a list of moves for a specific game
        Task<List<Move>> GetMovesForGame(int gameId);

        // Adds a user to a game (for linking games to users)
        Task AddUserToGame(int gameId, int userId);

        // Get a game by user ID (to allow user-specific game retrieval)
        Task<IEnumerable<Game>> GetGamesByUserId(int userId);

        // Add a new user
        Task<User> AddUser(User user);

        // Get a user by ID
        Task<User?> GetUserById(int userId);

        // Check if a user exists by ID
        Task<bool> UserExists(int userId);

        // Check if a username exists
        Task<bool> UsernameExists(string username);

        // Get a user by their username
        Task<User?> GetUserByUsername(string username);
    }
}
