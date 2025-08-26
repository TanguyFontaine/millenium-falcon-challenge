
// The key String is the planet
// The value List<int> is the list of days on which bounty hunters are on the planet.
using BountyHuntersMap = System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<int>>;

public class Pathfinder
{
    private UniverseGraphRepository m_universeGraphRepository;

    // Not used for the moment. Remains empty.
    private BountyHuntersMap m_bountyHuntersMap = new BountyHuntersMap();

    //private int m_falconAutonomy { get; set; }

    // Constructor
    public Pathfinder(UniverseGraphRepository universeGraphRepository)
    {
        m_universeGraphRepository = universeGraphRepository;
    }

    // Method to find the shortest path
    // Returns the number of days to go from Planet A to Planet B without exceeding the daysCountdown.
    public int FindShortestPath(string startPlanetName, string arrivalPlanetName, int daysCountdown)
    {
        Planet? startPlanet = m_universeGraphRepository.FindPlanet(startPlanetName);
        Planet? arrivalPlanet = m_universeGraphRepository.FindPlanet(arrivalPlanetName);

        return FindShortestPath(startPlanet, arrivalPlanet, daysCountdown);
    }

    private int FindShortestPath(Planet? startPlanet, Planet? arrivalPlanet, int daysCountdown)
    {
        int daysToArrival = -1;

        if (startPlanet == null || arrivalPlanet == null)
        {
            // Planet not found.
            return daysToArrival;
        }

        // The shortest distance found from startPlanet to each other planet.
        var distances = new Dictionary<Planet, int>();

        // Planets to visit.
        var unvisited = new List<Planet>(m_universeGraphRepository.GetAllPlanets());

        foreach (var planet in m_universeGraphRepository.GetAllPlanets())
        {
            distances[planet] = int.MaxValue;
        }
        distances[startPlanet] = 0;

        while (unvisited.Count > 0)
        {
            // pick the planet with smallest distance
            unvisited.Sort((a, b) => distances[a].CompareTo(distances[b]));
            Planet currentPlanet = unvisited[0];
            unvisited.RemoveAt(0);

            if (currentPlanet == arrivalPlanet)
                break;

            foreach (var neighbor in currentPlanet.Neighbors)
            {
                int newDistance = distances[currentPlanet] + neighbor.Value;
                if (newDistance < distances[neighbor.Key])
                {
                    distances[neighbor.Key] = newDistance;
                }
            }
        }

        if (distances[arrivalPlanet] != int.MaxValue && distances[arrivalPlanet] <= daysCountdown)
        {
            daysToArrival = distances[arrivalPlanet];
        }

        return daysToArrival;
    } 
}