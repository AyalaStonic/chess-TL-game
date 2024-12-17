using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace ChessBackend
{
    [ApiController]
    [Route("api/[controller]")]
    public class GameController : ControllerBase
    {
        private static string connectionString = "Server=your_server_name;Database=ChessGame;Trusted_Connection=True;";

        [HttpPost]
        public IActionResult SaveMove([FromBody] GameMove move)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                var query = "INSERT INTO GameMoves (FromSquare, ToSquare, Piece, GameId, UserId) VALUES (@FromSquare, @ToSquare, @Piece, @GameId, @UserId)";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FromSquare", move.FromSquare);
                    command.Parameters.AddWithValue("@ToSquare", move.ToSquare);
                    command.Parameters.AddWithValue("@Piece", move.Piece);
                    command.Parameters.AddWithValue("@GameId", move.GameId);
                    command.Parameters.AddWithValue("@UserId", move.UserId);

                    command.ExecuteNonQuery();
                }
            }

            return Ok("Move saved successfully!");
        }

        [HttpGet("{gameId}")]
        public IActionResult GetMoves(int gameId)
        {
            var moves = new List<GameMove>();

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                var query = "SELECT * FROM GameMoves WHERE GameId = @GameId ORDER BY MoveTime";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@GameId", gameId);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            moves.Add(new GameMove
                            {
                                Id = (int)reader["Id"],
                                MoveTime = (DateTime)reader["MoveTime"],
                                FromSquare = reader["FromSquare"].ToString(),
                                ToSquare = reader["ToSquare"].ToString(),
                                Piece = reader["Piece"].ToString(),
                                GameId = (int)reader["GameId"],
                                UserId = (int)reader["UserId"]
                            });
                        }
                    }
                }
            }

            return Ok(moves);
        }

        [HttpGet]
        [Route("games")]
        public IActionResult GetAllGames()
        {
            var games = new List<Game>();

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                var query = "SELECT DISTINCT GameId FROM GameMoves";
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            games.Add(new Game
                            {
                                GameId = (int)reader["GameId"]
                            });
                        }
                    }
                }
            }

            return Ok(games);
        }
    }

    public class GameMove
    {
        public int Id { get; set; }
        public DateTime MoveTime { get; set; }
        public string FromSquare { get; set; }
        public string ToSquare { get; set; }
        public string Piece { get; set; }
        public int GameId { get; set; }
        public int UserId { get; set; }
    }

    public class Game
    {
        public int GameId { get; set; }
    }
}
