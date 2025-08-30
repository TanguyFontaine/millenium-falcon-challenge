using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class UniverseGraphRepositoryFactoryTests
{
    [TestMethod]
    public void Build_SingleRoute_CreatesTwoPlanetsWithBidirectionalConnection()
    {
        var routes = new List<RouteDto>
        {
            new RouteDto { m_origin = "Alderaan", m_destination = "Coruscant", m_distance = 5 }
        };

        UniverseGraphRepository actualRepository = UniverseGraphRepositoryFactory.Build(routes);

        Planet? alderaan = actualRepository.FindPlanet("Alderaan");
        Planet? coruscant = actualRepository.FindPlanet("Coruscant");

        Assert.IsNotNull(alderaan);
        Assert.IsNotNull(coruscant);

        // Verify bidirectional connection
        Assert.IsTrue(alderaan.Neighbors.ContainsKey(coruscant.Name));
        Assert.IsTrue(coruscant.Neighbors.ContainsKey(alderaan.Name));
        Assert.AreEqual(5, alderaan.Neighbors[coruscant.Name]);
        Assert.AreEqual(5, coruscant.Neighbors[alderaan.Name]);
    }

    [TestMethod]
    public void Build_ValidSimpleRoutes()
    {
        var routes = new List<RouteDto>
        {
            new RouteDto { m_origin = "Tatooine", m_destination = "Dagobah", m_distance = 6 },
            new RouteDto { m_origin = "Dagobah", m_destination = "Endor", m_distance = 4 },
            new RouteDto { m_origin = "Dagobah", m_destination = "Hoth", m_distance = 1 }
        };

        UniverseGraphRepository actualRepository = UniverseGraphRepositoryFactory.Build(routes);

        // Verify planets exist
        Planet? tatooine = actualRepository.FindPlanet("Tatooine");
        Planet? dagobah = actualRepository.FindPlanet("Dagobah");
        Planet? endor = actualRepository.FindPlanet("Endor");
        Planet? hoth = actualRepository.FindPlanet("Hoth");

        Assert.IsNotNull(tatooine);
        Assert.IsNotNull(dagobah);
        Assert.IsNotNull(endor);
        Assert.IsNotNull(hoth);

        // Verify connections are bidirectional
        Assert.IsTrue(tatooine.Neighbors.ContainsKey(dagobah.Name));
        Assert.IsTrue(dagobah.Neighbors.ContainsKey(tatooine.Name));
        Assert.AreEqual(6, tatooine.Neighbors[dagobah.Name]);
        Assert.AreEqual(6, dagobah.Neighbors[tatooine.Name]);

        Assert.IsTrue(dagobah.Neighbors.ContainsKey(endor.Name));
        Assert.IsTrue(endor.Neighbors.ContainsKey(dagobah.Name));
        Assert.AreEqual(4, dagobah.Neighbors[endor.Name]);
        Assert.AreEqual(4, endor.Neighbors[dagobah.Name]);

        Assert.IsTrue(dagobah.Neighbors.ContainsKey(hoth.Name));
        Assert.IsTrue(hoth.Neighbors.ContainsKey(dagobah.Name));
        Assert.AreEqual(1, dagobah.Neighbors[hoth.Name]);
        Assert.AreEqual(1, hoth.Neighbors[dagobah.Name]);
    }

    [TestMethod]
    public void Build_MultipleRoutesToSamePlanet_GroupsCorrectly()
    {
        var routes = new List<RouteDto>
        {
            new RouteDto { m_origin = "Tatooine", m_destination = "Hoth", m_distance = 6 },
            new RouteDto { m_origin = "Tatooine", m_destination = "Dagobah", m_distance = 6 },
            new RouteDto { m_origin = "Hoth", m_destination = "Dagobah", m_distance = 1 },
            new RouteDto { m_origin = "Dagobah", m_destination = "Endor", m_distance = 4 },
            new RouteDto { m_origin = "Hoth", m_destination = "Endor", m_distance = 1 }
        };

        UniverseGraphRepository actualRepository = UniverseGraphRepositoryFactory.Build(routes);

        // Verify planets exist
        Planet? tatooine = actualRepository.FindPlanet("Tatooine");
        Planet? dagobah = actualRepository.FindPlanet("Dagobah");
        Planet? endor = actualRepository.FindPlanet("Endor");
        Planet? hoth = actualRepository.FindPlanet("Hoth");

        Assert.IsNotNull(tatooine);
        Assert.IsNotNull(dagobah);
        Assert.IsNotNull(endor);
        Assert.IsNotNull(hoth);

        // Verify tatooine has two neighbors
        Assert.IsTrue(tatooine.Neighbors.ContainsKey(hoth.Name));
        Assert.IsTrue(tatooine.Neighbors.ContainsKey(dagobah.Name));
        Assert.AreEqual(6, tatooine.Neighbors[hoth.Name]);
        Assert.AreEqual(6, tatooine.Neighbors[dagobah.Name]);

        // Verify hoth has three neighbors
        Assert.IsTrue(hoth.Neighbors.ContainsKey(tatooine.Name));
        Assert.IsTrue(hoth.Neighbors.ContainsKey(dagobah.Name));
        Assert.IsTrue(hoth.Neighbors.ContainsKey(endor.Name));
        Assert.AreEqual(6, hoth.Neighbors[tatooine.Name]);
        Assert.AreEqual(1, hoth.Neighbors[dagobah.Name]);
        Assert.AreEqual(1, hoth.Neighbors[endor.Name]);

        // Verify dagobah has three neighbors
        Assert.IsTrue(dagobah.Neighbors.ContainsKey(tatooine.Name));
        Assert.IsTrue(dagobah.Neighbors.ContainsKey(hoth.Name));
        Assert.IsTrue(dagobah.Neighbors.ContainsKey(endor.Name));
        Assert.AreEqual(6, dagobah.Neighbors[tatooine.Name]);
        Assert.AreEqual(1, dagobah.Neighbors[hoth.Name]);
        Assert.AreEqual(4, dagobah.Neighbors[endor.Name]);

        // Verify endor has two neighbors
        Assert.IsTrue(endor.Neighbors.ContainsKey(dagobah.Name));
        Assert.IsTrue(endor.Neighbors.ContainsKey(hoth.Name));
        Assert.AreEqual(4, endor.Neighbors[dagobah.Name]);
        Assert.AreEqual(1, endor.Neighbors[hoth.Name]);
    }

    [TestMethod]
    public void Build_EmptyRoutesList_CreatesEmptyRepository()
    {
        var routes = new List<RouteDto>();

        UniverseGraphRepository actualRepository = UniverseGraphRepositoryFactory.Build(routes);

        // Repository should exist but be empty
        Assert.IsNotNull(actualRepository);
        Assert.IsTrue(actualRepository.IsEmpty());
    }

    [TestMethod]
    public void Build_DuplicateRoutesWithDifferentDistances_UsesLatestDistance()
    {
        // Maybe the factory should ignore it instead and log a warning, or throw an exception

        var routes = new List<RouteDto>
        {
            new RouteDto { m_origin = "Tatooine", m_destination = "Dagobah", m_distance = 6 },
            new RouteDto { m_origin = "Dagobah", m_destination = "Tatooine", m_distance = 5 },
            new RouteDto { m_origin = "Tatooine", m_destination = "Dagobah", m_distance = 4 }  
        };

        UniverseGraphRepository actualRepository = UniverseGraphRepositoryFactory.Build(routes);

        Planet? tatooine = actualRepository.FindPlanet("Tatooine");
        Planet? dagobah = actualRepository.FindPlanet("Dagobah");

        Assert.IsNotNull(tatooine);
        Assert.IsNotNull(dagobah);

        Assert.IsTrue(tatooine.Neighbors.ContainsKey(dagobah.Name));
        Assert.IsTrue(dagobah.Neighbors.ContainsKey(tatooine.Name));
        Assert.AreEqual(4, tatooine.Neighbors[dagobah.Name]);
        Assert.AreEqual(4, dagobah.Neighbors[tatooine.Name]);
    }

    [TestMethod]
    public void Build_ZeroDistanceRoute_AddsPlanetsButNoConnection()
    {
        var routes = new List<RouteDto>
        {
            new RouteDto { m_origin = "Naboo", m_destination = "Theed", m_distance = 0 }
        };

        UniverseGraphRepository actualRepository = UniverseGraphRepositoryFactory.Build(routes);

        Planet? naboo = actualRepository.FindPlanet("Naboo");
        Planet? theed = actualRepository.FindPlanet("Theed");

        Assert.IsNotNull(naboo);
        Assert.IsNotNull(theed);

        Assert.IsFalse(naboo.Neighbors.ContainsKey(theed.Name));
        Assert.IsFalse(theed.Neighbors.ContainsKey(naboo.Name));
    }

    [TestMethod]
    public void Build_SamePlanetAsOriginAndDestination_CreatesOnePlanet()
    {
        var routes = new List<RouteDto>
        {
            new RouteDto { m_origin = "DeathStar", m_destination = "DeathStar", m_distance = 0 }
        };

        UniverseGraphRepository actualRepository = UniverseGraphRepositoryFactory.Build(routes);

        Planet? deathStar = actualRepository.FindPlanet("DeathStar");

        Assert.IsNotNull(deathStar);
        Assert.IsFalse(deathStar.Neighbors.ContainsKey(deathStar.Name));
    }

    [TestMethod]
    public void Build_LargeNetwork_CreatesComplexGraph()
    {
        var routes = new List<RouteDto>
        {
            new RouteDto { m_origin = "Tatooine", m_destination = "Alderaan", m_distance = 3 },
            new RouteDto { m_origin = "Alderaan", m_destination = "Coruscant", m_distance = 2 },
            new RouteDto { m_origin = "Coruscant", m_destination = "Naboo", m_distance = 1 },
            new RouteDto { m_origin = "Naboo", m_destination = "Kamino", m_distance = 4 },
            new RouteDto { m_origin = "Kamino", m_destination = "Geonosis", m_distance = 5 },
            new RouteDto { m_origin = "Geonosis", m_destination = "Tatooine", m_distance = 2 },
            new RouteDto { m_origin = "Coruscant", m_destination = "Kamino", m_distance = 3 }
        };

        UniverseGraphRepository actualRepository = UniverseGraphRepositoryFactory.Build(routes);

        // Verify all planets exist
        string[] expectedPlanets = { "Tatooine", "Alderaan", "Coruscant", "Naboo", "Kamino", "Geonosis" };
        foreach (string planetName in expectedPlanets)
        {
            Planet? planet = actualRepository.FindPlanet(planetName);
            Assert.IsNotNull(planet, $"Planet {planetName} should exist in the repository");
        }

        // Get all planets for connection verification
        Planet tatooine = actualRepository.FindPlanet("Tatooine")!;
        Planet alderaan = actualRepository.FindPlanet("Alderaan")!;
        Planet coruscant = actualRepository.FindPlanet("Coruscant")!;
        Planet naboo = actualRepository.FindPlanet("Naboo")!;
        Planet kamino = actualRepository.FindPlanet("Kamino")!;
        Planet geonosis = actualRepository.FindPlanet("Geonosis")!;

        // Verify Tatooine connections: Alderaan (3), Geonosis (2)
        Assert.AreEqual(2, tatooine.Neighbors.Count, "Tatooine should have 2 neighbors");
        Assert.IsTrue(tatooine.Neighbors.ContainsKey(alderaan.Name));
        Assert.IsTrue(tatooine.Neighbors.ContainsKey(geonosis.Name));
        Assert.AreEqual(3, tatooine.Neighbors[alderaan.Name]);
        Assert.AreEqual(2, tatooine.Neighbors[geonosis.Name]);

        // Verify Alderaan connections: Tatooine (3), Coruscant (2)
        Assert.AreEqual(2, alderaan.Neighbors.Count, "Alderaan should have 2 neighbors");
        Assert.IsTrue(alderaan.Neighbors.ContainsKey(tatooine.Name));
        Assert.IsTrue(alderaan.Neighbors.ContainsKey(coruscant.Name));
        Assert.AreEqual(3, alderaan.Neighbors[tatooine.Name]);
        Assert.AreEqual(2, alderaan.Neighbors[coruscant.Name]);

        // Verify Coruscant connections: Alderaan (2), Naboo (1), Kamino (3)
        Assert.AreEqual(3, coruscant.Neighbors.Count, "Coruscant should have 3 neighbors");
        Assert.IsTrue(coruscant.Neighbors.ContainsKey(alderaan.Name));
        Assert.IsTrue(coruscant.Neighbors.ContainsKey(naboo.Name));
        Assert.IsTrue(coruscant.Neighbors.ContainsKey(kamino.Name));
        Assert.AreEqual(2, coruscant.Neighbors[alderaan.Name]);
        Assert.AreEqual(1, coruscant.Neighbors[naboo.Name]);
        Assert.AreEqual(3, coruscant.Neighbors[kamino.Name]);

        // Verify Naboo connections: Coruscant (1), Kamino (4)
        Assert.AreEqual(2, naboo.Neighbors.Count, "Naboo should have 2 neighbors");
        Assert.IsTrue(naboo.Neighbors.ContainsKey(coruscant.Name));
        Assert.IsTrue(naboo.Neighbors.ContainsKey(kamino.Name));
        Assert.AreEqual(1, naboo.Neighbors[coruscant.Name]);
        Assert.AreEqual(4, naboo.Neighbors[kamino.Name]);

        // Verify Kamino connections: Naboo (4), Geonosis (5), Coruscant (3)
        Assert.AreEqual(3, kamino.Neighbors.Count, "Kamino should have 3 neighbors");
        Assert.IsTrue(kamino.Neighbors.ContainsKey(naboo.Name));
        Assert.IsTrue(kamino.Neighbors.ContainsKey(geonosis.Name));
        Assert.IsTrue(kamino.Neighbors.ContainsKey(coruscant.Name));
        Assert.AreEqual(4, kamino.Neighbors[naboo.Name]);
        Assert.AreEqual(5, kamino.Neighbors[geonosis.Name]);
        Assert.AreEqual(3, kamino.Neighbors[coruscant.Name]);

        // Verify Geonosis connections: Kamino (5), Tatooine (2)
        Assert.AreEqual(2, geonosis.Neighbors.Count, "Geonosis should have 2 neighbors");
        Assert.IsTrue(geonosis.Neighbors.ContainsKey(kamino.Name));
        Assert.IsTrue(geonosis.Neighbors.ContainsKey(tatooine.Name));
        Assert.AreEqual(5, geonosis.Neighbors[kamino.Name]);
        Assert.AreEqual(2, geonosis.Neighbors[tatooine.Name]);
    }
}
