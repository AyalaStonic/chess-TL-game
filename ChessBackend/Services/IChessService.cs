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
        Task AddMove(int gameId, Move move);  // Async method for adding a move

        // Start a new game
        Task<Game> StartNewGame();

        // Resets the current game
        Task<Game> ResetGame(int gameId);  // Reset a specific game

        // Marks a game as completed (updates status and sets EndedAt)
        Task CompleteGame(int gameId);

        // Save the game state (update game in the database)
        Task SaveGame(Game game);

        Task<bool> ValidateMove(int gameId, Move move);

        // Returns a list of moves for a specific game
        Task<List<Move>> GetMovesForGame(int gameId);
    }
}
