
// The key String is the planet
// The value List<int> is the list of days on which bounty hunters are on the planet.
using BountyHuntersMap = System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<int>>;

// For testing and debug purpose, keep track of the data returned by the Pathfinder class.
// m_numberOfDays -> number of days it took to travel from start to arrival planet.
// m_successProbability -> probability of success if the path crosses bounty hunters.
    // 0 if the shortest path takes longer than the countdown limit.
    // 100 if we can reach the arrival planet before the countdown without encountering bounty hunters.
public struct PathData
{
    public PathData(int numberOfDays, double successProbability)
    {
        m_numberOfDays = numberOfDays;
        m_successProbability = successProbability;
    }

    public int m_numberOfDays { get; }
    public double m_successProbability { get; }
}
public class Pathfinder
{
    private UniverseGraphRepository m_universeGraphRepository;

    private BountyHuntersMap m_bountyHuntersMap = new BountyHuntersMap();

    private int m_falconAutonomy { get; set; }

    // Constructor
    public Pathfinder(UniverseGraphRepository universeGraphRepository, BountyHuntersMap bountyHunters, int falconAutonomy)
    {
        m_universeGraphRepository = universeGraphRepository;
        m_bountyHuntersMap = bountyHunters;
        m_falconAutonomy = falconAutonomy;
    }

    // Method to find the shortest path
    // Returns the probability of reaching the destination within the time limit.
    // 0 if the shortest path takes longer than the time limit.
    // 100 if we can reach the arrival planet before the countdownwithout encountering bounty hunters.
    public PathData FindShortestPath(string startPlanetName, string arrivalPlanetName, int daysCountdown)
    {
        Planet? startPlanet = m_universeGraphRepository.FindPlanet(startPlanetName);
        Planet? arrivalPlanet = m_universeGraphRepository.FindPlanet(arrivalPlanetName);

        return FindShortestPath(startPlanet, arrivalPlanet, daysCountdown);
    }

    private bool planetHasBountyHuntersAtGivenDay(Planet planet, int day)
    {
        if (m_bountyHuntersMap.TryGetValue(planet.Name, out var bountyHuntersDaysPresence))
        {
            return bountyHuntersDaysPresence.Contains(day);
        }
        return false;
    }

    private double CalculateSuccessProbability(int bountyHuntersEncounters)
    {
        if (bountyHuntersEncounters == 0)
            return 100;

        double gettingCaughtProbability = 0;
        for (int i = 1; i <= bountyHuntersEncounters; i++)
        {
            gettingCaughtProbability += 100 * (Math.Pow(9, i - 1) / Math.Pow(10, i));
        }

        return 100 - gettingCaughtProbability;
    }

    private PathData FindShortestPath(Planet? startPlanet, Planet? arrivalPlanet, int daysCountdown)
    {
        if (startPlanet == null || arrivalPlanet == null)
        {
            // Planet not found.
            PathData defaultPathData = new PathData(-1, 0);
            return defaultPathData;
        }

        double successProbability = 0;
        int shortestDaysToArrival = int.MaxValue;

        // Map planets with fuel state -> minimum days to reach it
        var distances = new Dictionary<(Planet planet, int remainingFuel, int bountyHuntersEncounters), int>();

        var startState = (startPlanet, m_falconAutonomy, 0);
        distances[startState] = 0;

        // Planets to visit. Starting with startPlanet
        // Priority queue with the state (Planet, RemainingFuel, BountyHuntersEncounters, Days)
        var unvisited = new List<(Planet planet, int remainingFuel, int bountyHuntersEncounters, int days)>();
        unvisited.Add((startPlanet, m_falconAutonomy, 0, 0));

        while (unvisited.Count > 0)
        {
            // Sort unvisited planets by their distance and pick the closest one.
            unvisited.Sort((a, b) => a.days.CompareTo(b.days));
            var current = unvisited[0];
            unvisited.RemoveAt(0);

            Planet currentPlanet = current.planet;
            int currentFuel = current.remainingFuel;
            int currentBountyHuntersEncounters = current.bountyHuntersEncounters;
            int currentDays = current.days;

            // Skip if there already is a better path to this planet.
            var currentState = (currentPlanet, currentFuel, currentBountyHuntersEncounters);
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

                int totalTravelTime = currentDays;
                int newFuelForCurrentRoute = currentFuel;

                // Check if we need to refuel before traveling.
                if (currentFuel < daysToNeighbor)
                {
                    newFuelForCurrentRoute = m_falconAutonomy;
                    totalTravelTime += 1; // Refueling takes 1 day.

                    // Check for bounty hunters when refueling.
                    if (planetHasBountyHuntersAtGivenDay(currentPlanet, totalTravelTime))
                    {
                        currentBountyHuntersEncounters++;
                    }
                }

                // Travel to neighbor planet.
                totalTravelTime += daysToNeighbor;
                newFuelForCurrentRoute -= daysToNeighbor;

                var newState = (neighborPlanet, newFuelForCurrentRoute, currentBountyHuntersEncounters);
                if (!distances.ContainsKey(newState) || distances[newState] > totalTravelTime)
                {
                    if (planetHasBountyHuntersAtGivenDay(neighborPlanet, totalTravelTime))
                    {
                        currentBountyHuntersEncounters++;
                    }
                    // Update the shortest distance to this neighbor and add it to the unvisited queue.
                    distances[newState] = totalTravelTime;
                    unvisited.Add((neighborPlanet, newFuelForCurrentRoute, currentBountyHuntersEncounters, totalTravelTime));
                }
            }
        }

        // Find the minimum days to reach arrival planet.
        foreach (var state in distances.Keys)
        {
            if (state.planet == arrivalPlanet && distances[state] < shortestDaysToArrival)
            {
                shortestDaysToArrival = distances[state];
                if (shortestDaysToArrival <= daysCountdown)
                    successProbability = CalculateSuccessProbability(state.bountyHuntersEncounters);
            }
        }

        PathData result = new PathData(shortestDaysToArrival == int.MaxValue || shortestDaysToArrival > daysCountdown
                        ? -1
                        : shortestDaysToArrival
                        , successProbability);
        return result;
    }
}