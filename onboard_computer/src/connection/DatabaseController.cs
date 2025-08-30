using Microsoft.AspNetCore.Mvc;
using System.Data.SQLite;

[ApiController]
[Route("api/[controller]")]
public class DatabaseController : ControllerBase
{
    // Retrieves all planets data from a specific database file path
    public async Task<List<object>> GetUniverseRoutesAsync(string dbFilePath)
    {
        if (string.IsNullOrEmpty(dbFilePath) || !System.IO.File.Exists(dbFilePath))
        {
            throw new FileNotFoundException($"Database file not found: {dbFilePath}");
        }

        var routes = new List<object>();
        string connectionString = $"Data Source={dbFilePath};";

        using (var connection = new SQLiteConnection(connectionString))
        {
            await connection.OpenAsync();

            string query = "SELECT * FROM routes";

            using (var command = new SQLiteCommand(query, connection))
            {
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        routes.Add(new
                        {
                            origin = reader["ORIGIN"]?.ToString() ?? string.Empty,
                            destination = reader["DESTINATION"]?.ToString() ?? string.Empty,
                            distance = reader["TRAVEL_TIME"] != DBNull.Value ? Convert.ToInt32(reader["TRAVEL_TIME"]) : 0
                        });
                    }
                }
            }
        }

        return routes;
    }
}
