// ChessService.cs
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace ChessBackend.Services
{
    public class ChessService
    {
        private readonly string _connectionString;

        // Constructor that accepts connection string from app settings
        public ChessService(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Save a move to the database
        public async Task SaveMoveAsync(string gameId, string move)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var query = "INSERT INTO GameMoves (GameId, Move) VALUES (@gameId, @move)";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@gameId", gameId);
                command.Parameters.AddWithValue("@move", move);

                await command.ExecuteNonQueryAsync();
            }
        }

        // Get all moves for a specific game
        public async Task<List<string>> GetGameMovesAsync(string gameId)
        {
            var moves = new List<string>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                var query = "SELECT Move FROM GameMoves WHERE GameId = @gameId";
                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@gameId", gameId);

                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        moves.Add(reader.GetString(0));
                    }
                }
            }

            return moves;
        }
    }
}
