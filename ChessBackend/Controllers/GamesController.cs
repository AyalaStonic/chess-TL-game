using ChessBackend.Models;
using ChessBackend.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ChessBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        private readonly IChessService _chessService;

        public GamesController(IChessService chessService)
        {
            _chessService = chessService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllGames()
        {
            var games = await _chessService.GetAllGamesAsync();
            return Ok(games);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetGameById(int id)
        {
            var game = await _chessService.GetGameByIdAsync(id);
            if (game == null)
            {
                return NotFound();
            }
            return Ok(game);
        }

        [HttpPost]
        public async Task<IActionResult> AddGame([FromBody] Game game)
        {
            if (game == null)
            {
                return BadRequest();
            }

            await _chessService.CreateGameAsync(game); // Change made here
            return CreatedAtAction(nameof(GetGameById), new { id = game.Id }, game);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGame(int id, [FromBody] Game game)
        {
            if (game == null || id != game.Id)
            {
                return BadRequest();
            }

            await _chessService.UpdateGameAsync(game);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGame(int id)
        {
            await _chessService.DeleteGameAsync(id);
            return NoContent();
        }
    }
}
