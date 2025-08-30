
using System.Text.Json;
public class EmpireDataParser
{
    public static EmpireDataDto Parse(string jsonContent)
    {
        try
        {
            EmpireDataDto? empireDto = JsonSerializer.Deserialize<EmpireDataDto>(jsonContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (empireDto == null)
            {
                throw new InvalidDataException("Failed to read empire data - invalid JSON structure");
            }

            return empireDto;
        }
        catch (JsonException ex)
        {
            throw new ArgumentException($"Invalid JSON format: {ex.Message}", ex);
        }
    }
}
