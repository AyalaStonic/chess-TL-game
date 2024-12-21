using Microsoft.AspNetCore.Mvc;
using ChessBackend.Models;
using ChessBackend.Services;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;  // Import async Task

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
        public async Task<IActionResult> GetGames()
        {
            try
            {
                var games = await _chessService.GetAllGames(); // Ensure async call

                // Check if games are available
                if (games == null || !games.Any())
                {
                    return NotFound(new { message = "No games found." }); // Return 404 with a message if no games are found
                }

                // Return the games in a 200 OK response
                return Ok(games); // Return the list of games in JSON format
            }
            catch (Exception ex)
            {
                // Return 500 Internal Server Error with an error message
                return StatusCode(500, new { message = "An error occurred while fetching the games.", error = ex.Message });
            }
        }

        // GET api/chess/games/{id}
        [HttpGet("games/{id}")]
        public async Task<IActionResult> GetGame(int id)
        {
            try
            {
                var game = await _chessService.GetGameById(id); // Ensure async call
                if (game == null)
                {
                    return NotFound(new { message = "Game not found." }); // Return 404 if the game with the specified ID is not found
                }
                return Ok(game); // Return 200 OK with the game details
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching the game.", error = ex.Message });
            }
        }

        // GET api/chess/games/{id}/moves
        [HttpGet("games/{id}/moves")]
        public async Task<IActionResult> GetMoves(int id)
        {
            try
            {
                var game = await _chessService.GetGameById(id); // Ensure async call
                if (game == null)
                {
                    return NotFound(new { message = "Game not found." });
                }

                var moves = await _chessService.GetMovesForGame(id); // Fetch moves for the game
                return Ok(moves); // Return 200 OK with the list of moves
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching moves.", error = ex.Message });
            }
        }

        // POST api/chess/move
        [HttpPost("move")]
        public async Task<IActionResult> MakeMove([FromQuery] int gameId, [FromBody] Move move)
        {
            try
            {
                var game = await _chessService.GetGameById(gameId); // Ensure async call
                if (game == null)
                {
                    return NotFound(new { message = "Game not found." });
                }

                // Validate the move and apply it
                var isValidMove = await _chessService.ValidateMove(gameId, move); // Ensure ValidateMove method is async
                if (!isValidMove)
                {
                    return BadRequest(new { message = "Invalid move." });
                }

                await _chessService.AddMove(gameId, move); // Add the move to the game
                return Ok(new { message = "Move added successfully", game }); // Return the updated game
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Invalid move.", error = ex.Message });
            }
        }

        // POST api/chess/games
        [HttpPost("games")]
        public async Task<IActionResult> AddGame([FromBody] Game game)
        {
            if (game == null)
            {
                return BadRequest(new { message = "Invalid game data." }); // Return 400 if the provided game data is invalid
            }

            // Optional: Validate model before adding it
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Return validation errors if any
            }

            try
            {
                await _chessService.AddGame(game); // Ensure async call
                return CreatedAtAction(nameof(GetGame), new { id = game.Id }, game); // Return 201 Created with the location of the newly created game
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while adding the game.", error = ex.Message });
            }
        }

        // POST api/chess/start
        [HttpPost("start")]
        public async Task<IActionResult> StartNewGame()
        {
            try
            {
                var newGame = await _chessService.StartNewGame(); // Ensure async call
                return CreatedAtAction(nameof(GetGame), new { id = newGame.Id }, newGame); // Return 201 Created with the new game
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Could not start a new game.", error = ex.Message });
            }
        }

        // POST api/chess/reset/{gameId} - Reset the game
        [HttpPost("reset/{gameId}")]
        public async Task<IActionResult> ResetGame(int gameId)
        {
            try
            {
                var resetGame = await _chessService.ResetGame(gameId); // Ensure async call
                return Ok(resetGame); // Return the reset game object
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = $"Game with ID {gameId} not found." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while resetting the game.", error = ex.Message });
            }
        }

        // POST api/chess/save - Save the game state
        [HttpPost("save")]
        public async Task<IActionResult> SaveGame([FromBody] Game game)
        {
            if (game == null)
            {
                return BadRequest(new { message = "Invalid game data." }); // Return 400 if the game data is not valid
            }

            try
            {
                await _chessService.SaveGame(game); // Ensure async call
                return Ok(new { message = "Game saved successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error saving game", error = ex.Message }); // Return 500 error if something goes wrong
            }
        }
    }
}
