
// The key String is the planet
// The value List<int> is the list of days on which bounty hunters are on the planet.
using BountyHuntersMap = System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<int>>;

public class Pathfinder
{
    private UniverseGraphRepository m_universeGraphRepository;

    // Not used for the moment. Remains empty.
    private BountyHuntersMap m_bountyHuntersMap = new BountyHuntersMap();

    private int m_falconAutonomy { get; set; }

    // Constructor
    public Pathfinder(UniverseGraphRepository universeGraphRepository, int falconAutonomy)
    {
        m_universeGraphRepository = universeGraphRepository;
        m_falconAutonomy = falconAutonomy;
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
        if (startPlanet == null || arrivalPlanet == null)
        {
            // Planet not found.
            return -1;
        }

        int daysToArrival = int.MaxValue;

        // Map planets with fuel state -> minimum days to reach it
        var distances = new Dictionary<(Planet planet, int remainingFuel), int>();
        
        var startState = (startPlanet, m_falconAutonomy);
        distances[startState] = 0;

        // Planets to visit. Starting with startPlanet
        // Priority queue with the state (Planet, RemainingFuel, Days)
        var unvisited = new List<(Planet planet, int remainingFuel, int days)>();
        unvisited.Add((startPlanet, m_falconAutonomy, 0));

        while (unvisited.Count > 0)
        {
            // Sort unvisited planets by their distance and pick the closest one.
            unvisited.Sort((a, b) => a.days.CompareTo(b.days));
            var current = unvisited[0];
            unvisited.RemoveAt(0);

            Planet currentPlanet = current.planet;
            int currentFuel = current.remainingFuel;
            int currentDays = current.days;

            // Skip if there already is a better path to this planet.
            var currentState = (currentPlanet, currentFuel);
            if (distances.ContainsKey(currentState) && distances[currentState] < currentDays)
                continue;

            // Explore neighbors
            foreach (var neighbor in currentPlanet.Neighbors)
            {
                Planet neighborPlanet = neighbor.Key;
                int daysToNeighbor = neighbor.Value;

                // Can't travel if route exceeds fuel capacity.
                if (daysToNeighbor > m_falconAutonomy)
                    continue;

                int totalTravelTime = currentDays + daysToNeighbor;
                int newFuelForCurrentRoute = currentFuel;

                // Check if we need to refuel before traveling.
                if (currentFuel < daysToNeighbor)
                {
                    newFuelForCurrentRoute = m_falconAutonomy;
                    totalTravelTime += 1; // Refueling takes 1 day.
                }

                newFuelForCurrentRoute -= daysToNeighbor;
                var newState = (neighborPlanet, newFuelForCurrentRoute);
                if (!distances.ContainsKey(newState) || distances[newState] > totalTravelTime)
                {
                    // Update the shortest distance to this neighbor and add it to the unvisited queue.
                    distances[newState] = totalTravelTime;
                    unvisited.Add((neighborPlanet, newFuelForCurrentRoute, totalTravelTime));
                }
            }
        }

        // Find the minimum days to reach arrival planet.
        foreach (var state in distances.Keys)
        {
            if (state.planet == arrivalPlanet && distances[state] < daysToArrival)
            {
                daysToArrival = distances[state];
            }
        }

        return (daysToArrival == int.MaxValue) || (daysToArrival > daysCountdown)
                ? -1
                : daysToArrival;
    }
}