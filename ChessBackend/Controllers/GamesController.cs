using Microsoft.AspNetCore.Mvc;
using ChessBackend.Models;
using ChessBackend.Services;
using System.Linq;
using System.Collections.Generic;

namespace ChessBackend.Controllers
{
    [Route("api/chess")]  // Route for all chess-related endpoints
    [ApiController]
    public class GamesController : ControllerBase
    {
        private readonly IChessService _chessService;

        public GamesController(IChessService chessService)
        {
            _chessService = chessService;
        }

        // GET api/chess/games
        [HttpGet("games")]
        public IActionResult GetGames()
        {
            var games = _chessService.GetAllGames();
            if (games == null || !games.Any())
            {
                return NotFound(); // Return 404 if no games are found
            }
            return Ok(games); // Return 200 OK with the games list
        }

        // GET api/chess/games/{id}
        [HttpGet("games/{id}")]
        public IActionResult GetGame(int id)
        {
            var game = _chessService.GetGameById(id);
            if (game == null)
            {
                return NotFound(); // Return 404 if the game with the specified ID is not found
            }
            return Ok(game); // Return 200 OK with the game details
        }

        // GET api/chess/games/{id}/moves
        [HttpGet("games/{id}/moves")]
        public IActionResult GetMoves(int id)
        {
            var game = _chessService.GetGameById(id);
            if (game == null)
            {
                return NotFound(new { message = "Game not found." });
            }

            var moves = _chessService.GetMovesForGame(id); // Fetch moves for the game
            return Ok(moves); // Return 200 OK with the list of moves
        }

        // POST api/chess/move
        [HttpPost("move")]
        public IActionResult MakeMove([FromQuery] int gameId, [FromBody] string move)
        {
            var game = _chessService.GetGameById(gameId);
            if (game == null)
            {
                return NotFound(new { message = "Game not found." });
            }

            try
            {
                // Validate the move and apply it
                _chessService.AddMove(gameId, move); // Add the move to the game
                return Ok(new { message = "Move added successfully", game }); // Return the updated game
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Invalid move.", error = ex.Message });
            }
        }

        // POST api/chess/games
        [HttpPost("games")]
        public IActionResult AddGame([FromBody] Game game)
        {
            if (game == null)
            {
                return BadRequest(); // Return 400 if the provided game data is invalid
            }

            // Optional: Validate model before adding it
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Return validation errors if any
            }

            _chessService.AddGame(game);
            return CreatedAtAction(nameof(GetGame), new { id = game.Id }, game); // Return 201 Created with the location of the newly created game
        }

        // POST api/chess/start
        [HttpPost("start")]
        public IActionResult StartNewGame()
        {
            var newGame = _chessService.StartNewGame();
            if (newGame == null)
            {
                return BadRequest("Could not start a new game."); // Handle any failure
            }

            return CreatedAtAction(nameof(GetGame), new { id = newGame.Id }, newGame); // Return 201 Created with the new game
        }

        // POST api/chess/reset/{gameId} - Reset the game
        [HttpPost("reset/{gameId}")]
        public IActionResult ResetGame(int gameId)
        {
            try
            {
                var resetGame = _chessService.ResetGame(gameId);
                return Ok(resetGame); // Return the reset game object
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = $"Game with ID {gameId} not found." });
            }
        }
        
        // POST api/chess/save - Save the game state
        [HttpPost("save")]
        public IActionResult SaveGame([FromBody] Game game)
        {
            if (game == null)
            {
                return BadRequest("Invalid game data."); // Return 400 if the game data is not valid
            }

            try
            {
                _chessService.SaveGame(game); // Call SaveGame method in the service
                return Ok(new { message = "Game saved successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Error saving game", error = ex.Message }); // Return 400 with error message
            }
        }
    }
}
