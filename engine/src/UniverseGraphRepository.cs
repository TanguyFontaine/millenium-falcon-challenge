using System;
using System.Collections.Generic;

// A Node in the graph representing the universe.
public class Planet
{
    public string Name { get; set; }
    public Dictionary<string, int> Neighbors { get; set; } = new Dictionary<string, int>();

    public Planet(string name)
    {
        Name = name;
    }

    public void AddNeighbor(Planet node, int distance)
    {
        Neighbors[node.Name] = distance;
    }

    // Compare planets by name.
    public override bool Equals(object? obj)
    {
        return obj is Planet planet && Name.Equals(planet.Name, StringComparison.OrdinalIgnoreCase);
    }

    public override int GetHashCode()
    {
        return Name?.GetHashCode(StringComparison.OrdinalIgnoreCase) ?? 0;
    }
}

public class UniverseGraphRepository
{
    private HashSet<Planet> m_planets = new HashSet<Planet>();

    public void AddPlanet(Planet planet)
    {
        m_planets.Add(planet);
    }

    public Planet? FindPlanet(string name)
    {
        return m_planets.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    public IEnumerable<Planet> GetAllPlanets()
    {
        return m_planets;
    }

    public bool IsEmpty()
    {
        return !m_planets.Any();
    }
}