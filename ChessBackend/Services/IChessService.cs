using ChessBackend.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChessBackend.Services
{
    public interface IChessService
    {
        Task<Game> GetGameByIdAsync(int id);  // Change to non-nullable return type
        Task CreateGameAsync(Game game);  // AddGameAsync method
        Task UpdateGameAsync(Game game);
        Task DeleteGameAsync(int id);
        Task<IEnumerable<Game>> GetAllGamesAsync();  // Changed return type to IEnumerable
    }
}
