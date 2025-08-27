using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class PathfinderTests
{
    private UniverseGraphRepository CreateTestRepository()
    {
        var repository = new UniverseGraphRepository();

        // Create test planets
        var tatooine = new Planet("Tatooine");
        var dagobah = new Planet("Dagobah");
        var endor = new Planet("Endor");
        var hoth = new Planet("Hoth");

        // Create connections between planets
        tatooine.AddNeighbor(dagobah, 6);
        tatooine.AddNeighbor(hoth, 6);
        dagobah.AddNeighbor(endor, 4);
        dagobah.AddNeighbor(hoth, 1);
        hoth.AddNeighbor(endor, 1);
        endor.AddNeighbor(dagobah, 4);
        hoth.AddNeighbor(dagobah, 1);
        endor.AddNeighbor(hoth, 1);

        // Add planets to repository
        repository.AddPlanet(tatooine);
        repository.AddPlanet(dagobah);
        repository.AddPlanet(endor);
        repository.AddPlanet(hoth);

        return repository;
    }

    private UniverseGraphRepository CreateTestRepository2()
    {
        var repository = new UniverseGraphRepository();

        // Create test planets
        var tatooine = new Planet("Tatooine");
        var dagobah = new Planet("Dagobah");
        var endor = new Planet("Endor");
        var hoth = new Planet("Hoth");

        // Create connections between planets
        tatooine.AddNeighbor(dagobah, 4);
        tatooine.AddNeighbor(hoth, 5);
        dagobah.AddNeighbor(endor, 5);
        dagobah.AddNeighbor(hoth, 3);
        hoth.AddNeighbor(endor, 2);
        endor.AddNeighbor(dagobah, 5);
        hoth.AddNeighbor(dagobah, 3);
        endor.AddNeighbor(hoth, 2);

        // Add planets to repository
        repository.AddPlanet(tatooine);
        repository.AddPlanet(dagobah);
        repository.AddPlanet(endor);
        repository.AddPlanet(hoth);

        return repository;
    }

    [TestMethod]
    public void FindShortestPath_NonExistentPlanet_ReturnsMinusOne()
    {
        var repository = CreateTestRepository();
        var pathfinder = new Pathfinder(repository, 6);

        int resultStartNonExistent = pathfinder.FindShortestPath("NonExistent", "Endor", 10);
        int resultArrivalNonExistent = pathfinder.FindShortestPath("Tatooine", "NonExistent", 10);

        Assert.AreEqual(-1, resultStartNonExistent);
        Assert.AreEqual(-1, resultArrivalNonExistent);
    }

    [TestMethod]
    public void FindShortestPath_SamePlanet_ReturnsZero()
    {
        var repository = CreateTestRepository();
        var pathfinder = new Pathfinder(repository, 6);

        int result = pathfinder.FindShortestPath("Tatooine", "Tatooine", 10);

        Assert.AreEqual(0, result);
    }

    [TestMethod]
    public void FindShortestPath_ValidPlanetsAndCountdownBigEnough_ReturnsShortestDistance()
    {
        var repository = CreateTestRepository();
        var pathfinder = new Pathfinder(repository, 6);

        int result = pathfinder.FindShortestPath("Tatooine", "Endor", 10);

        // Tatooine -> Hoth (6 + 1) -> Endor (1) = 8 days with refueling
        Assert.AreEqual(8, result);
    }

    [TestMethod]
    public void FindShortestPath_ValidPlanetsOnOtherRepository_ReturnsShortestDistance()
    {
        var repository = CreateTestRepository2();
        var pathfinder = new Pathfinder(repository, 6);

        int result = pathfinder.FindShortestPath("Tatooine", "Endor", 10);

        // Tatooine -> Hoth (5 + 1) -> Endor (2) = 8 days with refueling
        Assert.AreEqual(8, result);
    }

    [TestMethod]
    public void FindShortestPath_RefuelingMakesCountdownToShort_ReturnsMinusOne()
    {
        var repository = CreateTestRepository();
        var pathfinder = new Pathfinder(repository, 6);

        int result = pathfinder.FindShortestPath("Tatooine", "Endor", 7);

        // Tatooine -> Hoth (6 + 1) -> Endor (1) = 8 days with refueling
        Assert.AreEqual(-1, result);
    }

    [TestMethod]
    public void FindShortestPath_ArrivingSameDayAsCountdown_ReturnsShortestDistance()
    {
        var repository = CreateTestRepository();
        var pathfinder = new Pathfinder(repository, 6);

        int result = pathfinder.FindShortestPath("Tatooine", "Endor", 8);

        // Tatooine -> Hoth (6 + 1) -> Endor (1) = 8 days with refueling
        Assert.AreEqual(8, result);
    }

    [TestMethod]
    public void FindShortestPath_CountdownNotBigEnough_ReturnsMinusOne()
    {
        var repository = CreateTestRepository();
        var pathfinder = new Pathfinder(repository, 6);

        int result = pathfinder.FindShortestPath("Tatooine", "Endor", 5);

        // Tatooine -> Hoth (6 + 1) -> Endor (1) = 8 days with refueling
        Assert.AreEqual(-1, result);
    }

    [TestMethod]
    public void FindShortestPath_TwoOtherValidPlanets_ReturnsShortestDistance()
    {
        var repository = CreateTestRepository();
        var pathfinder = new Pathfinder(repository, 6);

        int result = pathfinder.FindShortestPath("Dagobah", "Hoth", 3);

        // Dagobah -> Hoth (1) = 1 day
        Assert.AreEqual(1, result);
    }
    
        [TestMethod]
    public void FindShortestPath_FuelCapacityTooSmall_ReturnsMinusOne()
    {
        var repository = CreateTestRepository();
        var pathfinder = new Pathfinder(repository, 3);

        int result = pathfinder.FindShortestPath("Tatooine", "Endor", 15);

        // Both Tatooine neighbors require more fuel than available
        Assert.AreEqual(-1, result);
    }
}
