using ChessBackend.Models;
using ChessBackend.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChessBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GamesController : ControllerBase
    {
        private readonly ChessService _chessService;

        public GamesController(ChessService chessService)
        {
            _chessService = chessService;
        }

        [HttpGet("api/chess/games")]
        public IActionResult GetAllGames()
        {
            try
            {
                var games = _chessService.GetAllGames();
                return Ok(games);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpPost("CreateGame")]
        public IActionResult CreateGame([FromBody] Game game)
        {
            try
            {
                _chessService.CreateGame(game);
                return Ok(game);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        // Endpoint to replay the moves of a specific game
        [HttpGet("ReplayGame/{gameId}")]
        public IActionResult ReplayGame(int gameId)
        {
            try
            {
                var moves = _chessService.GetMovesForGame(gameId);
                return Ok(moves);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }
    }
}
