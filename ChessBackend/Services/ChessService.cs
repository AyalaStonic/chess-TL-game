using ChessBackend.Data;
using ChessBackend.Models;
using Microsoft.EntityFrameworkCore;

namespace ChessBackend.Services
{
    public class ChessService : IChessService
    {
        private readonly AppDbContext _context;

        public ChessService(AppDbContext context)
        {
            _context = context;
        }

        // Implement the GetGameByIdAsync method with non-nullable return type
        public async Task<Game> GetGameByIdAsync(int id)
        {
            var game = await _context.Games.FirstOrDefaultAsync(g => g.Id == id);
            if (game == null)
                throw new KeyNotFoundException("Game not found");

            return game;
        }

        // Implement CreateGameAsync (was missing)
        public async Task CreateGameAsync(Game game)
        {
            _context.Games.Add(game);
            await _context.SaveChangesAsync();
        }

        // Implement UpdateGameAsync
        public async Task UpdateGameAsync(Game game)
        {
            _context.Games.Update(game);
            await _context.SaveChangesAsync();
        }

        // Implement DeleteGameAsync
        public async Task DeleteGameAsync(int id)
        {
            var game = await _context.Games.FirstOrDefaultAsync(g => g.Id == id);
            if (game != null)
            {
                _context.Games.Remove(game);
                await _context.SaveChangesAsync();
            }
        }

        // Implement GetAllGamesAsync with matching return type
        public async Task<IEnumerable<Game>> GetAllGamesAsync()
        {
            return await _context.Games.ToListAsync();
        }
    }
}
