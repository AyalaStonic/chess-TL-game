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
        private readonly ChessDbContext _context;

        // Constructor accepting both dependencies
        public GamesController(IChessService chessService, ChessDbContext context)
        {
            _chessService = chessService;
            _context = context;
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
public IActionResult MakeMove([FromBody] MoveData moveData)
{
    // Validate the incoming data
    if (moveData == null || string.IsNullOrEmpty(moveData.From) || string.IsNullOrEmpty(moveData.To))
    {
        return BadRequest("Invalid move data.");
    }

    // Fetch the game from the database
    var game = _context.Games.Find(moveData.GameId);
    if (game == null)
    {
        return NotFound("Game not found.");
    }

    // Load the current game state using ChessDotNet.ChessGame
    var chessGame = new ChessDotNet.ChessGame(game.Fen);

    // Convert 'From' and 'To' to ChessDotNet.Position
    var fromPosition = new ChessDotNet.Position(moveData.From);
    var toPosition = new ChessDotNet.Position(moveData.To);

    // Create the move using ChessDotNet.Move
    var chessMove = new ChessDotNet.Move(fromPosition, toPosition, chessGame.WhoseTurn);

    // Use MakeMove to validate and apply the move
    ChessDotNet.MoveType moveResult = chessGame.MakeMove(chessMove, true);  // 'true' for alreadyValidated flag
    
    // Check if the move was successfully applied
    if (moveResult == ChessDotNet.MoveType.Invalid)
    {
        return BadRequest("Invalid move.");
    }

    // Update the game's FEN string
    game.Fen = chessGame.GetFen();

    // Create a new database move data record
    var moveDataEntry = new MoveData
    {
        GameId = moveData.GameId,
        From = moveData.From,
        To = moveData.To
    };

    // Save MoveData to the database first
    _context.MoveData.Add(moveDataEntry);  // Save the MoveData entry
    _context.SaveChanges();  // Ensure MoveData gets an Id assigned

    // Create a new Move record that references the MoveData
    var moveEntry = new ChessBackend.Models.Move
    {
        GameId = moveData.GameId,
        MoveOrder = _context.Moves.Count(m => m.GameId == moveData.GameId) + 1, // Increment move order for the game
        MoveDataId = moveDataEntry.Id, // Link to the saved MoveData
        PlayedAt = DateTime.UtcNow // Assign the current timestamp
    };

    // Save the Move to the database
    _context.Moves.Add(moveEntry);

    // Save the updated game state to the database
    _context.SaveChanges();

    // Return the updated FEN to the frontend
    return Ok(new { success = true, fen = game.Fen });
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

    
// POST: api/chess/start/{userId}
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

        // Check if the game was successfully created
        if (newGame == null)
        {
            return StatusCode(500, new { message = "Could not start a new game." });
        }

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
    // Validate the request
    if (loginRequest == null || string.IsNullOrWhiteSpace(loginRequest.Username))
    {
        return BadRequest(new { message = "Invalid login request. Username is required." });
    }

    try
    {
        // Attempt to fetch the user by username
        var user = await _chessService.GetUserByUsername(loginRequest.Username);
        if (user == null)
        {
            return NotFound(new { message = "User not found." });
        }

        // Return user details (id and username) for successful login
        return Ok(new
        {
            message = "Login successful.",
            user = new
            {
                id = user.Id,
                username = user.Username
            }
        });
    }
    catch (Exception ex)
    {
        // Log the exception (optional: use a logging service here)
        Console.Error.WriteLine($"Error during login: {ex.Message}");

        // Return a generic server error response
        return StatusCode(500, new
        {
            message = "An error occurred while processing the login request.",
            error = ex.Message
        });
    }
}}}
