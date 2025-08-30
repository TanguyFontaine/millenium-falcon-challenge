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

    // To use the default config file
    [HttpGet("read")]
    public async Task<IActionResult> ReadDefaultFalconConfiguration()
    {
        return await ProcessFalconConfigurationFromFile(OnboardComputerConfig.MillenniumFalconFilePath);
    }

    // To use uploaded config content from C3PO
    [HttpPost("read")]
    public async Task<IActionResult> ReadFalconConfigurationFromContent([FromBody] JsonElement falconConfigJson)
    {
        return await ProcessFalconConfiguration(falconConfigJson.GetRawText(), isUploadedContent: true);
    }

    private async Task<IActionResult> ProcessFalconConfigurationFromFile(string configPath)
    {
        if (!System.IO.File.Exists(configPath))
        {
            return NotFound(new { message = $"Falcon configuration file not found: {configPath}" });
        }

        string jsonContent = await System.IO.File.ReadAllTextAsync(configPath);
        return await ProcessFalconConfiguration(jsonContent, isUploadedContent: false, configPath);
    }

    private async Task<IActionResult> ProcessFalconConfiguration(string falconJsonContent, bool isUploadedContent, string? configFilePath = null)
    {
        try
        {
            JsonDocument jsonDocument = JsonDocument.Parse(falconJsonContent);
            JsonElement root = jsonDocument.RootElement;

            // Extract configuration values
            int autonomy = GetIntValue(root, "autonomy");
            string departure = GetStringValue(root, "departure");
            string arrival = GetStringValue(root, "arrival");
            string databasePath = GetStringValue(root, "routes_db");

            // uploaded file database path must be absolute.
            // TODO: enforce that and check with error logging
            string configurationDirectoryPath = isUploadedContent ?
                                                "" :
                                                Path.GetDirectoryName(Path.GetFullPath(OnboardComputerConfig.MillenniumFalconFilePath)) ?? "";
            string completeDatabasePath = Path.Combine(configurationDirectoryPath, databasePath);

            // Call DatabaseController to read all universe routes
            var universeRoutes = await m_databaseController.GetUniverseRoutesAsync(completeDatabasePath);

            return Ok(new
            {
                autonomy = autonomy,
                departure = departure,
                arrival = arrival,
                routes_data = universeRoutes,
                source = configFilePath ?? "uploaded JSON content"
            });
        }
        catch (JSONValidationException ex)
        {
            return BadRequest(new
            {
                message = "Error reading Millennium Falcon data: invalid JSON format",
                error = ex.Message,
                source = configFilePath ?? "uploaded JSON content"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "Error reading Millennium Falcon configuration.",
                error = ex.Message,
                source = configFilePath ?? "uploaded JSON content"
            });
        }
    }
}
