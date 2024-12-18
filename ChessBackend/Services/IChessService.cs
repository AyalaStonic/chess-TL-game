using ChessBackend.Models;
using System.Collections.Generic;

namespace ChessBackend.Services
{
    public interface IChessService
    {
        // Returns a list of all games
        IEnumerable<Game> GetAllGames();
        
        // Returns a game by its ID, nullable because it can return null if the game is not found
        Game? GetGameById(int id);  
        
        // Adds a new game to the collection
        void AddGame(Game game);
        
        // Adds a move to an existing game
        void AddMove(int gameId, string move);
    }
}
