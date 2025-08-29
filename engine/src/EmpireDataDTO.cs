
using System.Text.Json;
using System.Text.Json.Serialization;

// DTO for JSON deserialization
public class EmpireDataDto : IEquatable<EmpireDataDto>
{
    [JsonPropertyName("countdown")]
    public int m_countdown { get; set; }
    
    [JsonPropertyName("bounty_hunters")]
    public List<BountyHunterDto> m_bountyHunters { get; set; } = new List<BountyHunterDto>();

    public bool Equals(EmpireDataDto? other)
    {
        if (other == null)
            return false;

        if (ReferenceEquals(this, other))
            return true;
        
        return m_countdown == other.m_countdown &&
               m_bountyHunters.SequenceEqual(other.m_bountyHunters);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as EmpireDataDto);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(m_countdown, m_bountyHunters.Count);
    }
}

public class BountyHunterDto : IEquatable<BountyHunterDto>
{
    [JsonPropertyName("planet")]
    public string m_planet { get; set; } = string.Empty;
    
    [JsonPropertyName("day")]
    public int m_day { get; set; }

    public bool Equals(BountyHunterDto? other)
    {
        if (other == null)
            return false;
        if (ReferenceEquals(this, other))
            return true;

        return m_planet == other.m_planet && m_day == other.m_day;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as BountyHunterDto);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(m_planet, m_day);
    }
}

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
