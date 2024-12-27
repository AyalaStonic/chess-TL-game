using ChessBackend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChessBackend.Services
{
    public interface IChessService
    {
        Task<IEnumerable<Game>> GetAllGames();
        Task<Game?> GetGameById(int id);
        Task AddGame(Game game);
        Task AddMove(int gameId, Move move);
        Task<Game> StartNewGame(int userId);
        Task<Game> ResetGame(int gameId);
        Task CompleteGame(int gameId);
        Task SaveGame(Game game);
        Task<bool> ValidateMove(int gameId, Move move);
        Task<List<Move>> GetMovesForGame(int gameId);
        Task AddUserToGame(int gameId, int userId);
        Task<IEnumerable<Game>> GetGamesByUserId(int userId);
        Task<User> AddUser(User user);
        Task<User?> GetUserById(int userId);
        Task<bool> UserExists(int userId);
        Task<bool> UsernameExists(string username);
        Task<User?> GetUserByUsername(string username);
         string UndoMove(Game game);
    }
}
