using Microsoft.AspNetCore.Mvc;
using ChessBackend.Models;
using ChessBackend.Services;
using System.Linq;

namespace ChessBackend.Controllers
{
    // Ensure the correct route prefix is set
    [Route("api/chess/[controller]")]
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

        // POST api/chess/games
        [HttpPost("games")]
        public IActionResult AddGame([FromBody] Game game)
        {
            if (game == null)
            {
                return BadRequest(); // Return 400 if the provided game data is invalid
            }

            _chessService.AddGame(game);
            return CreatedAtAction(nameof(GetGame), new { id = game.Id }, game); // Return 201 Created with the location of the newly created game
        }

        // POST api/chess/games/{gameId}/moves
        [HttpPost("games/{gameId}/moves")]
        public IActionResult AddMove(int gameId, [FromBody] string move)
        {
            var game = _chessService.GetGameById(gameId);
            if (game == null)
            {
                return NotFound(); // Return 404 if the game with the specified ID is not found
            }

            _chessService.AddMove(gameId, move);
            return Ok(game); // Return 200 OK with the updated game details
        }
    }
}
