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
        // Fetch the game by its ID
        var game = await _chessService.GetGameById(gameId);

        if (game == null)
        {
            return NotFound(new { message = "Game not found." });
        }

        // Validate the move (ensure moveData exists and is valid)
        if (move.MoveData == null || string.IsNullOrEmpty(move.MoveData.From) || string.IsNullOrEmpty(move.MoveData.To))
        {
            return BadRequest(new { message = "Invalid move data. 'from' and 'to' must be provided." });
        }

        var isValidMove = await _chessService.ValidateMove(gameId, move);

        if (!isValidMove)
        {
            return BadRequest(new { message = "Invalid move." });
        }

        // Add the move to the game (this method should handle the logic for adding the move)
        await _chessService.AddMove(gameId, move);

        return Ok(new { message = "Move added successfully", game });
    }
    catch (Exception ex)
    {
        // Return an error if something goes wrong
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

    
/// POST: api/chess/start/{userId}
[HttpPost("start/{userId}")]
public async Task<IActionResult> StartNewGame(int userId)
{
    try
    {
        // Validate the user exists before starting a new game
        var userExists = await _chessService.UserExists(userId);
        if (!userExists)
        {
            return NotFound(new { message = "User not found." });
        }

        // Start the game using the userId
        var newGame = await _chessService.StartNewGame(userId);

        // Return the newly created game, using CreatedAtAction to return the location of the new resource
        return CreatedAtAction(nameof(GetGame), new { id = newGame.Id }, newGame);
    }
    catch (ArgumentException ex)
    {
        // Handle specific exceptions such as invalid arguments
        return BadRequest(new { message = "Invalid request.", error = ex.Message });
    }
    catch (Exception ex)
    {
        // Log and return a 500 status with error message in case of unexpected failures
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

       [HttpPost("user")]
        public async Task<IActionResult> AddUser([FromBody] User user)
        {
            if (user == null || string.IsNullOrWhiteSpace(user.Username))
            {
                return BadRequest(new { message = "Invalid user data. Username is required." });
            }

            try
            {
                // Ensure the username is unique
                var usernameExists = await _chessService.UsernameExists(user.Username);
                if (usernameExists)
                {
                    return Conflict(new { message = "Username already exists." });
                }

                // Add the user
                var createdUser = await _chessService.AddUser(user);

                // Return the created user resource
                return CreatedAtAction(nameof(GetUser), new { id = createdUser.Id }, createdUser);
            }
            catch (Exception ex)
            {
                // Log the error and return a server error
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
                // Log the error and return a server error
                return StatusCode(500, new { message = "An error occurred while retrieving the user.", error = ex.Message });
            }
        }

        // POST: api/chess/user/login
        [HttpPost("user/login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            if (loginRequest == null || string.IsNullOrEmpty(loginRequest.Username))
            {
                return BadRequest(new { message = "Invalid login request. Username is required." });
            }

            try
            {
                var user = await _chessService.GetUserByUsername(loginRequest.Username);
                if (user == null)
                {
                    return Unauthorized(new { message = "User not found." });
                }

                return Ok(new { message = "Login successful.", user });
            }
            catch (Exception ex)
            {
                // Log the error and return a server error
                return StatusCode(500, new { message = "An error occurred while processing the login request.", error = ex.Message });
            }
        }
    }}