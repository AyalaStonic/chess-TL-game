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

        [HttpGet("GetAllGames")]
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
    }
}
