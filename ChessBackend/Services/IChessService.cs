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

        // Start a new game
        Game StartNewGame();
        
        // Resets the current game
        Game ResetGame(int gameId);  // Reset a specific game
        
        // Marks a game as completed (updates status and sets EndedAt)
        void CompleteGame(int gameId);

        // Save the game state (update game in the database)
        void SaveGame(Game game);
        
        // Returns a list of moves for a specific game
        List<Move> GetMovesForGame(int gameId);
    }
}
