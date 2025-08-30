
using System.Text.Json.Serialization;

// DTO for JSON deserialization
public class MillenniumFalconDataDto : IEquatable<MillenniumFalconDataDto>
{
    [JsonPropertyName("autonomy")]
    public int m_autonomy { get; set; }
    
    [JsonPropertyName("departure")]
    public string m_departure { get; set; } = string.Empty;

    [JsonPropertyName("arrival")]
    public string m_arrival { get; set; } = string.Empty;

    [JsonPropertyName("routes_data")]
    public List<RouteDto> m_routes { get; set; } = new List<RouteDto>();

    public bool Equals(MillenniumFalconDataDto? other)
    {
        if (other == null)
            return false;

        if (ReferenceEquals(this, other))
            return true;

        return m_autonomy == other.m_autonomy &&
               m_departure == other.m_departure &&
               m_arrival == other.m_arrival &&
               m_routes.SequenceEqual(other.m_routes);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as MillenniumFalconDataDto);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(m_autonomy, m_departure, m_arrival, m_routes.Count);
    }
}

public class RouteDto : IEquatable<RouteDto>
{
    [JsonPropertyName("origin")]
    public string m_origin { get; set; } = string.Empty;

    [JsonPropertyName("destination")]
    public string m_destination { get; set; } = string.Empty;

    [JsonPropertyName("distance")]
    public int m_distance { get; set; }

    public bool Equals(RouteDto? other)
    {
        if (other == null)
            return false;
        if (ReferenceEquals(this, other))
            return true;

        return m_origin == other.m_origin &&
               m_destination == other.m_destination &&
               m_distance == other.m_distance;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as RouteDto);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(m_origin, m_destination, m_distance);
    }
}