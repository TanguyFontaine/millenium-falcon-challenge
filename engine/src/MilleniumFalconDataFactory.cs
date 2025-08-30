public struct MilleniumFalconData
{
    public int m_autonomy { get; set; }
    public string m_departurePlanet { get; set; }
    public string m_arrivalPlanet { get; set; }
    public UniverseGraphRepository m_universe { get; set; }
}

public class MilleniumFalconDataFactory
{
    public static MilleniumFalconData Build(ref MilleniumFalconDataDto falconDataDto)
    {
        return new MilleniumFalconData
        {
            m_autonomy = falconDataDto.m_autonomy,
            m_departurePlanet = falconDataDto.m_departure,
            m_arrivalPlanet = falconDataDto.m_arrival,
            m_universe = UniverseGraphRepositoryFactory.Build(falconDataDto.m_routes)
        };
    }
}