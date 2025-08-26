using System;
using System.Collections.Generic;

// A Node in the graph representing the universe.
public class Planet
{
    public string Value { get; set; }
    public Dictionary<Planet, int> Neighbors { get; set; } = new Dictionary<Planet, int>();

    public Planet(string value)
    {
        Value = value;
    }

    public void AddNeighbor(Planet node, int distance)
    {
        Neighbors[node] = distance;
    }
}

public class UniverseGraphRepository
{
    private List<Planet> m_planets = new List<Planet>();

    public void AddPlanet(Planet planet)
    {
        m_planets.Add(planet);
    }

    public Planet? FindPlanet(string name)
    {
        return m_planets.Find(p => p.Value == name);
    }

    public IEnumerable<Planet> GetAllPlanets()
    {
        return m_planets;
    }
}