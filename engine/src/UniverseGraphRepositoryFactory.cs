public class UniverseGraphRepositoryFactory
{
    private static Planet GetOrCreatePlanet(UniverseGraphRepository repository, string planetName)
    {
        Planet? planet = repository.FindPlanet(planetName);
        if (planet == null)
        {
            planet = new Planet(planetName);
            repository.AddPlanet(planet);
        }
        return planet;
    }

    private static void AddBidirectionalRoute(Planet origin, Planet destination, int distance)
    {
        origin.AddNeighbor(destination, distance);
        destination.AddNeighbor(origin, distance);
    }

    public static UniverseGraphRepository Build(List<RouteDto> parsedDatabaseRoutes)
    {
        UniverseGraphRepository repository = new UniverseGraphRepository();

        foreach (RouteDto route in parsedDatabaseRoutes)
        {
            // If the route is not valid (zero distance or same planet)
            // The planets are still added to the repository but no connection is made.
            // Design decision, we could also completely ignore invalid routes.

            Planet origin = GetOrCreatePlanet(repository, route.m_origin);
            Planet destination = GetOrCreatePlanet(repository, route.m_destination);
            if (!origin.Equals(destination) && route.m_distance > 0)
            {
                AddBidirectionalRoute(origin, destination, route.m_distance);
            }
        }

        return repository;
    }
}
