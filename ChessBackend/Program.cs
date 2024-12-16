// Ensure that all using directives are at the top, before any class or code.
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;

namespace ChessBackend
{
    class Program
    {
        static string connectionString = "Server=your_server_name;Database=ChessGame;Trusted_Connection=True;";  // Change connection string

        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to the Chess Game Backend!");

            // Example move to add to the database
            var move = new GameMove
            {
                FromSquare = "e2",
                ToSquare = "e4",
                Piece = "p"
            };

            // Insert the move into the database
            InsertMove(move);

            // Get and display all moves
            var moves = GetAllMoves();
            foreach (var m in moves)
            {
                Console.WriteLine($"{m.MoveTime}: {m.Piece} from {m.FromSquare} to {m.ToSquare}");
            }
        }

        // Insert a move into the database
        static void InsertMove(GameMove move)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                var query = "INSERT INTO GameMoves (FromSquare, ToSquare, Piece) VALUES (@FromSquare, @ToSquare, @Piece)";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FromSquare", move.FromSquare);
                    command.Parameters.AddWithValue("@ToSquare", move.ToSquare);
                    command.Parameters.AddWithValue("@Piece", move.Piece);

                    command.ExecuteNonQuery();
                }
            }

            Console.WriteLine("Move inserted successfully!");
        }

        // Get all moves from the database
        static List<GameMove> GetAllMoves()
        {
            var moves = new List<GameMove>();

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                var query = "SELECT * FROM GameMoves";
                using (var command = new SqlCommand(query, connection))
                {
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
                                Piece = reader["Piece"].ToString()
                            });
                        }
                    }
                }
            }

            return moves;
        }
    }

    // Define the GameMove model
    public class GameMove
    {
        public int Id { get; set; }
        public DateTime MoveTime { get; set; }
        public string FromSquare { get; set; }
        public string ToSquare { get; set; }
        public string Piece { get; set; }
    }
}
