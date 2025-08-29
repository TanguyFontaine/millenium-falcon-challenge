using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

public class JSONValidationException : Exception
{
    public JSONValidationException(string message) : base(message) { }
}

[ApiController]
[Route("api/[controller]")]
public class OnboardComputerController : ControllerBase
{
    private DatabaseController m_databaseController;
    const string dummyFilePath = "..\\examples\\example1\\millennium-falcon.json";

    public OnboardComputerController()
    {
        m_databaseController = new DatabaseController();
    }

    private string GetStringValue(JsonElement rootElement, string propertyName)
    {
        if (!rootElement.TryGetProperty(propertyName, out JsonElement valueElement))
        {
            throw new JSONValidationException($"{propertyName} field not found");
        }

        string propertyValue = valueElement.GetString() ?? "";
        if (string.IsNullOrEmpty(propertyValue))
        {
            throw new JSONValidationException($"{propertyName} field is empty");
        }

        return propertyValue;
    }

    private int GetIntValue(JsonElement rootElement, string propertyName)
    {
        if (!rootElement.TryGetProperty(propertyName, out JsonElement valueElement))
        {
            throw new JSONValidationException($"{propertyName} field not found");
        }

        try
        {
            return valueElement.GetInt32();
        }
        catch (InvalidOperationException)
        {
            throw new JSONValidationException($"{propertyName} field is not a valid integer");
        }
    }

    [HttpGet("read")]
    public async Task<IActionResult> ReadFalconConfigurationAndUniverse()
    {
        try
        {
            // Read the millennium falcon configuration file
            if (!System.IO.File.Exists(dummyFilePath))
            {
                return NotFound(new { message = $"Falcon configuration file not found: {dummyFilePath}" });
            }

            string jsonContent = await System.IO.File.ReadAllTextAsync(dummyFilePath);
            JsonDocument jsonDocument = JsonDocument.Parse(jsonContent);

            // Extract configuration values
            int autonomy = GetIntValue(jsonDocument.RootElement, "autonomy");
            string departure = GetStringValue(jsonDocument.RootElement, "departure");
            string arrival = GetStringValue(jsonDocument.RootElement, "arrival");
            string databasePath = GetStringValue(jsonDocument.RootElement, "routes_db");

            // Create complete database path (works for both relative and absolute paths)
            string configurationDirectoryPath = Path.GetDirectoryName(Path.GetFullPath(dummyFilePath)) ?? "";
            string completeDatabasePath = Path.Combine(configurationDirectoryPath, databasePath);

            // Call DatabaseController to read all universe routes (use the complete path)
            // Will throw FileNotFoundException if the database file is not found.
            var universeRoutes = await m_databaseController.GetUniverseRoutesAsync(completeDatabasePath);

            // Return combined response as one JSON string
            return Ok(new
            {
                autonomy = autonomy,
                departure = departure,
                arrival = arrival,
                routes_data = universeRoutes
            });
        }
        catch (JSONValidationException ex)
        {
            // Handle our custom validation errors as BadRequest
            return BadRequest(new
            {
                message = "Error reading Millenium Falcon data: invalid JSON format",
                error = ex.Message,
                filePath = dummyFilePath
            });
        }
        catch (Exception ex)
        {
            // Unexpected errors
            return StatusCode(500, new
            {
                message = "Error reading Millenium Falcon configuration.",
                error = ex.Message,
                filePath = dummyFilePath
            });
        }
    }
}
