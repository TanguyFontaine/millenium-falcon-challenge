// The key String is the planet
// The value SortedSet<int> is the list of days on which bounty hunters are on the planet.
using BountyHuntersMap = System.Collections.Generic.Dictionary<string, System.Collections.Generic.SortedSet<int>>;

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
    /********  Private Members  ***********/
    private UniverseGraphRepository m_universeGraphRepository;
    private BountyHuntersMap m_bountyHuntersMap = new BountyHuntersMap();
    private int m_falconAutonomy { get; set; }

    /********  Public Methods  ***********/
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


    /********  Private Methods  ***********/
    // Struct to keep track of the state of explored planets
    private struct ExploredPlanetState
    {
        public ExploredPlanetState(Planet planet, int remainingFuel, int bountyHuntersEncounters, int days)
        {
            this.planet = planet;
            this.remainingFuel = remainingFuel;
            this.bountyHuntersEncounters = bountyHuntersEncounters;
            this.days = days;
        }
        public Planet planet { get; }
        public int remainingFuel { get; }
        public int bountyHuntersEncounters { get; }
        public int days { get; }
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

    private void ExploreNeighbours(Queue<ExploredPlanetState> planetsToExplore,
                                   ExploredPlanetState currentState,
                                   int daysCountdown)
    {
        Planet currentPlanet = currentState.planet;
        int currentFuel = currentState.remainingFuel;
        int currentBountyHuntersEncounters = currentState.bountyHuntersEncounters;
        int currentDays = currentState.days;

        foreach (var neighbor in currentPlanet.Neighbors)
        {
            Planet? neighborPlanet = m_universeGraphRepository.FindPlanet(neighbor.Key);
            if (neighborPlanet == null)
                continue;

            int daysToNeighbor = neighbor.Value;

            // Can't travel if route exceeds fuel capacity.
            if (daysToNeighbor > m_falconAutonomy)
                continue;

            int totalTravelTime = currentDays;
            int newFuelForCurrentRoute = currentFuel;
            int newBountyHuntersEncounters = currentBountyHuntersEncounters;

            // Check if we need to refuel before traveling.
            if (currentFuel < daysToNeighbor)
            {
                newFuelForCurrentRoute = m_falconAutonomy;
                totalTravelTime += 1; // Refueling takes 1 day.

                // Check for bounty hunters when refueling.
                if (planetHasBountyHuntersAtGivenDay(currentPlanet, totalTravelTime))
                {
                    newBountyHuntersEncounters++;
                }
            }

            // Travel to neighbor planet.
            totalTravelTime += daysToNeighbor;
            newFuelForCurrentRoute -= daysToNeighbor;

            // Check for bounty hunters when arriving at destination.
            if (planetHasBountyHuntersAtGivenDay(neighborPlanet, totalTravelTime))
            {
                newBountyHuntersEncounters++;
            }

            // Add this path to the queue if it doesn't exceed countdown
            if (totalTravelTime <= daysCountdown)
            {
                ExploredPlanetState newState = new ExploredPlanetState(neighborPlanet, newFuelForCurrentRoute, newBountyHuntersEncounters, totalTravelTime);
                planetsToExplore.Enqueue(newState);
            }
        }

        // Option to wait one day at current planet (if not already at max days)
        // This creates a new path and adds the current planet back to the queue.
        if (currentDays < daysCountdown)
        {
            int waitDays = currentDays + 1;
            int waitEncounters = currentBountyHuntersEncounters;
            int waitFuel = m_falconAutonomy; // Refuel while waiting.
            
            // Check for bounty hunters when waiting.
            if (planetHasBountyHuntersAtGivenDay(currentPlanet, waitDays))
            {
                waitEncounters++;
            }

            ExploredPlanetState waitState = new ExploredPlanetState(currentPlanet, currentFuel, waitEncounters, waitDays);
            planetsToExplore.Enqueue(waitState);
        }
    }

    private List<(int daysToArrival, int bountyHunterEncounters)> ComputeAllPathsToArrival(Planet startPlanet, Planet arrivalPlanet, int daysCountdown)
    {
        var allPathsToArrival = new List<(int daysToArrival, int bountyHunterEncounters)>();

        // Queue for BFS exploration: (Planet, RemainingFuel, BountyHuntersEncounters, Days)
        var planetsToExplore = new Queue<ExploredPlanetState>();
        planetsToExplore.Enqueue(new ExploredPlanetState(startPlanet, m_falconAutonomy, 0, 0));

        // Keep track of visited states to avoid infinite loops
        var visited = new HashSet<ExploredPlanetState>();

        while (planetsToExplore.Count > 0)
        {
            var current = planetsToExplore.Dequeue();
            Planet currentPlanet = current.planet;
            int currentBountyHuntersEncounters = current.bountyHuntersEncounters;
            int currentDays = current.days;

            // Skip if we exceed the countdown
            if (currentDays > daysCountdown)
                continue;

            // Skip if we've seen this exact state before (avoid infinite loops)
            if (visited.Contains(current))
                continue;
            visited.Add(current);

            if (currentPlanet == arrivalPlanet)
            {
                allPathsToArrival.Add((currentDays, currentBountyHuntersEncounters));
                continue; // Don't explore further from arrival planet
            }

            ExploreNeighbours(planetsToExplore, current, daysCountdown);
        }

        return allPathsToArrival;
    }

    private PathData FindBestPath(List<(int daysToArrival, int bountyHunterEncounters)> allPathsToArrival, int daysCountdown)
    {
        double bestSuccessProbability = 0;
        int minDaysToArrival = int.MaxValue;

        foreach (var path in allPathsToArrival)
        {
            double pathSuccessProbability = CalculateSuccessProbability(path.bountyHunterEncounters);

            if (pathSuccessProbability > bestSuccessProbability)
            {
                bestSuccessProbability = pathSuccessProbability;
                minDaysToArrival = path.daysToArrival;
            }
            else if (pathSuccessProbability == bestSuccessProbability && path.daysToArrival < minDaysToArrival)
            {
                minDaysToArrival = path.daysToArrival;
            }
        }

        PathData result = new PathData(minDaysToArrival == int.MaxValue || minDaysToArrival > daysCountdown
                        ? -1
                        : minDaysToArrival
                        , bestSuccessProbability);
        return result;
    }

    private PathData FindShortestPath(Planet? startPlanet, Planet? arrivalPlanet, int daysCountdown)
    {
        if (startPlanet == null || arrivalPlanet == null)
        {
            // Planet not found.
            PathData defaultPathData = new PathData(-1, 0);
            return defaultPathData;
        }

        var allPathsToArrival = ComputeAllPathsToArrival(startPlanet, arrivalPlanet, daysCountdown);

        PathData result = FindBestPath(allPathsToArrival, daysCountdown);
        return result;
    }
}