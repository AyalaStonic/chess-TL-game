// GamesController.cs
using Microsoft.AspNetCore.Mvc;
using ChessBackend.Services;
using System.Threading.Tasks;

namespace ChessBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        private readonly ChessService _chessService;

        // Constructor with dependency injection
        public GamesController(ChessService chessService)
        {
            _chessService = chessService;
        }

        // POST: api/games/save-move
        [HttpPost("save-move")]
        public async Task<IActionResult> SaveMove([FromBody] GameMoveRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.GameId) || string.IsNullOrEmpty(request.Move))
            {
                return BadRequest("Invalid request");
            }

            await _chessService.SaveMoveAsync(request.GameId, request.Move);
            return Ok("Move saved successfully");
        }

        // GET: api/games/get-moves/{gameId}
        [HttpGet("get-moves/{gameId}")]
        public async Task<IActionResult> GetGameMoves(string gameId)
        {
            if (string.IsNullOrEmpty(gameId))
            {
                return BadRequest("Game ID is required");
            }

            var moves = await _chessService.GetGameMovesAsync(gameId);
            return Ok(moves);
        }
    }

    // Helper class to hold request data for saving a move
    public class GameMoveRequest
    {
        public string GameId { get; set; }
        public string Move { get; set; }
    }
}
