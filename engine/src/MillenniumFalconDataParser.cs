
using System.Text.Json;
public class MillenniumFalconParser
{
    public static MillenniumFalconDataDto Parse(string jsonContent)
    {
        try
        {
            MillenniumFalconDataDto? millenniumFalconDto = JsonSerializer.Deserialize<MillenniumFalconDataDto>(jsonContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (millenniumFalconDto == null)
            {
                throw new InvalidDataException("Failed to read millennium falcon data - invalid JSON structure");
            }

            return millenniumFalconDto;
        }
        catch (JsonException ex)
        {
            throw new ArgumentException($"Invalid JSON format: {ex.Message}", ex);
        }
    }
}
