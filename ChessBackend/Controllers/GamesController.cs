using Microsoft.AspNetCore.Mvc;
using ChessBackend.Models;
using ChessBackend.Services;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using ChessDotNet;

namespace ChessBackend.Controllers
{
    [Route("api/chess")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        private readonly IChessService _chessService;

        public GamesController(IChessService chessService)
        {
            _chessService = chessService;
        }

        // GET: api/chess/games
        [HttpGet("games")]
        public async Task<IActionResult> GetGames()
        {
            try
            {
                var games = await _chessService.GetAllGames();

                if (games == null || !games.Any())
                {
                    return NotFound(new { message = "No games found." });
                }

                return Ok(games);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching the games.", error = ex.Message });
            }
        }

        // GET: api/chess/games/{id}
        [HttpGet("games/{id}")]
        public async Task<IActionResult> GetGame(int id)
        {
            try
            {
                var game = await _chessService.GetGameById(id);

                if (game == null)
                {
                    return NotFound(new { message = "Game not found." });
                }

                return Ok(game);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching the game.", error = ex.Message });
            }
        }

        // GET: api/chess/games/{id}/moves
        [HttpGet("games/{id}/moves")]
        public async Task<IActionResult> GetMoves(int id)
        {
            try
            {
                var moves = await _chessService.GetMovesForGame(id);

                if (moves == null || !moves.Any())
                {
                    return NotFound(new { message = "No moves found for this game." });
                }

                return Ok(moves);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching moves.", error = ex.Message });
            }
        }

        // POST: api/chess/move
        [HttpPost("move")]
        public async Task<IActionResult> MakeMove([FromQuery] int gameId, [FromBody] ChessBackend.Models.Move move)
        {
            try
            {
                var game = await _chessService.GetGameById(gameId);

                if (game == null)
                {
                    return NotFound(new { message = "Game not found." });
                }

                var isValidMove = await _chessService.ValidateMove(gameId, move);

                if (!isValidMove)
                {
                    return BadRequest(new { message = "Invalid move." });
                }

                await _chessService.AddMove(gameId, move);

                return Ok(new { message = "Move added successfully", game });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while making the move.", error = ex.Message });
            }
        }

        // POST: api/chess/games
        [HttpPost("games")]
        public async Task<IActionResult> AddGame([FromBody] Game game)
        {
            if (game == null)
            {
                return BadRequest(new { message = "Invalid game data." });
            }

            try
            {
                await _chessService.AddGame(game);
                return CreatedAtAction(nameof(GetGame), new { id = game.Id }, game);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while adding the game.", error = ex.Message });
            }
        }

        // POST: api/chess/start
        [HttpPost("start")]
        public async Task<IActionResult> StartNewGame([FromQuery] int userId)
        {
            try
            {
                var newGame = await _chessService.StartNewGame(userId);
                return CreatedAtAction(nameof(GetGame), new { id = newGame.Id }, newGame);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Could not start a new game.", error = ex.Message });
            }
        }

        // POST: api/chess/reset/{gameId}
        [HttpPost("reset/{gameId}")]
        public async Task<IActionResult> ResetGame(int gameId)
        {
            try
            {
                var resetGame = await _chessService.ResetGame(gameId);
                return Ok(resetGame);
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

        // POST: api/chess/save
        [HttpPost("save")]
        public async Task<IActionResult> SaveGame([FromBody] Game game)
        {
            if (game == null)
            {
                return BadRequest(new { message = "Invalid game data." });
            }

            try
            {
                await _chessService.SaveGame(game);
                return Ok(new { message = "Game saved successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while saving the game.", error = ex.Message });
            }
        }

        // POST: api/chess/user
        [HttpPost("user")]
        public async Task<IActionResult> AddUser([FromBody] User user)
        {
            if (user == null)
            {
                return BadRequest(new { message = "Invalid user data." });
            }

            try
            {
                var createdUser = await _chessService.AddUser(user);
                return CreatedAtAction(nameof(GetUser), new { id = createdUser.Id }, createdUser);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while adding the user.", error = ex.Message });
            }
        }

        // GET: api/chess/user/{id}
         [HttpGet("user/{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            try
            {
                var user = await _chessService.GetUserById(id);

                if (user == null)
                {
                    return NotFound(new { message = "User not found." });
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching the user.", error = ex.Message });
            }
        }

        // GET: api/chess/games/user/{userId}
         [HttpGet("games/user/{userId}")]
        public async Task<IActionResult> GetUserGames(int userId)
        {
            try
            {
                var games = await _chessService.GetGamesByUserId(userId);

                if (games == null || !games.Any())
                {
                    return NotFound(new { message = "No games found for this user." });
                }

                return Ok(games);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching games for the user.", error = ex.Message });
            }
        }
    }
}
