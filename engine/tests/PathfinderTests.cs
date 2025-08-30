using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using BountyHuntersMap = System.Collections.Generic.Dictionary<string, System.Collections.Generic.SortedSet<int>>;

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
        dagobah.AddNeighbor(tatooine, 6);
        dagobah.AddNeighbor(endor, 4);
        dagobah.AddNeighbor(hoth, 1);
        hoth.AddNeighbor(tatooine, 6);
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
        tatooine.AddNeighbor(dagobah, 9);
        tatooine.AddNeighbor(hoth, 4);
        dagobah.AddNeighbor(endor, 2);
        dagobah.AddNeighbor(hoth, 3);
        hoth.AddNeighbor(endor, 8);
        endor.AddNeighbor(dagobah, 2);
        hoth.AddNeighbor(dagobah, 3);
        endor.AddNeighbor(hoth, 8);

        // Add planets to repository
        repository.AddPlanet(tatooine);
        repository.AddPlanet(dagobah);
        repository.AddPlanet(endor);
        repository.AddPlanet(hoth);

        return repository;
    }

    [TestMethod]
    public void FindShortestPath_NonExistentPlanet_ReturnsMinusOneDayAndZeroPercentSuccess()
    {
        var repository = CreateTestRepository();
        var bountyHuntersDaysPresence = new BountyHuntersMap();
        var pathfinder = new Pathfinder(repository, bountyHuntersDaysPresence, 6);

        PathData resultStartNonExistent = pathfinder.FindShortestPath("NonExistent", "Endor", 10);
        PathData resultArrivalNonExistent = pathfinder.FindShortestPath("Tatooine", "NonExistent", 10);

        Assert.AreEqual(-1, resultStartNonExistent.m_numberOfDays);
        Assert.AreEqual(-1, resultArrivalNonExistent.m_numberOfDays);
        Assert.AreEqual(0, resultStartNonExistent.m_successProbability);
        Assert.AreEqual(0, resultArrivalNonExistent.m_successProbability);

    }

    [TestMethod]
    public void FindShortestPath_SamePlanet_ReturnsZeroDaysAndHundredPercentSuccess()
    {
        var repository = CreateTestRepository();
        var bountyHuntersDaysPresence = new BountyHuntersMap();
        var pathfinder = new Pathfinder(repository, bountyHuntersDaysPresence, 6);

        PathData result = pathfinder.FindShortestPath("Tatooine", "Tatooine", 10);

        Assert.AreEqual(0, result.m_numberOfDays);
        Assert.AreEqual(100, result.m_successProbability);
    }

    [TestMethod]
    public void FindShortestPath_ValidPlanetsAndCountdownBigEnough_NoBH_ReturnsShortestDistanceWithHundredPercent()
    {
        var repository = CreateTestRepository();
        var bountyHuntersDaysPresence = new BountyHuntersMap();
        var pathfinder = new Pathfinder(repository, bountyHuntersDaysPresence, 6);

        PathData result = pathfinder.FindShortestPath("Tatooine", "Endor", 10);

        // Tatooine -> Hoth (6 + 1) -> Endor (1) = 8 days with refueling
        Assert.AreEqual(8, result.m_numberOfDays);
        Assert.AreEqual(100, result.m_successProbability);
    }

    [TestMethod]
    public void FindShortestPath_ValidPlanetsOnOtherRepository_NoBH_ReturnsShortestDistanceWithHundredPercent()
    {
        var repository = CreateTestRepository2();
        var bountyHuntersDaysPresence = new BountyHuntersMap();
        var pathfinder = new Pathfinder(repository, bountyHuntersDaysPresence, 6);

        PathData result = pathfinder.FindShortestPath("Tatooine", "Endor", 10);

        // Tatooine -> Hoth (4) -> Dagobah (3 + 1) -> Endor (2) = 10 days with refueling
        Assert.AreEqual(10, result.m_numberOfDays);
        Assert.AreEqual(100, result.m_successProbability);
    }

    [TestMethod]
    public void FindShortestPath_RefuelingMakesCountdownToShort_ReturnsMinusOneWithZeroPercent()
    {
        var repository = CreateTestRepository();
        var bountyHuntersDaysPresence = new BountyHuntersMap();
        var pathfinder = new Pathfinder(repository, bountyHuntersDaysPresence, 6);

        PathData result = pathfinder.FindShortestPath("Tatooine", "Endor", 7);

        // Tatooine -> Hoth (6 + 1) -> Endor (1) = 8 days with refueling
        Assert.AreEqual(-1, result.m_numberOfDays);
        Assert.AreEqual(0, result.m_successProbability);

    }

    [TestMethod]
    public void FindShortestPath_ArrivingSameDayAsCountdown_NoBH_ReturnsShortestDistanceWithHundredPercent()
    {
        var repository = CreateTestRepository();
        var bountyHuntersDaysPresence = new BountyHuntersMap();
        var pathfinder = new Pathfinder(repository, bountyHuntersDaysPresence, 6);

        PathData result = pathfinder.FindShortestPath("Tatooine", "Endor", 8);

        // Tatooine -> Hoth (6 + 1) -> Endor (1) = 8 days with refueling
        Assert.AreEqual(8, result.m_numberOfDays);
        Assert.AreEqual(100, result.m_successProbability);
    }

    [TestMethod]
    public void FindShortestPath_CountdownNotBigEnough_ReturnsMinusOneWithZeroPercent()
    {
        var repository = CreateTestRepository();
        var bountyHuntersDaysPresence = new BountyHuntersMap();
        var pathfinder = new Pathfinder(repository, bountyHuntersDaysPresence, 6);

        PathData result = pathfinder.FindShortestPath("Tatooine", "Endor", 5);

        // Tatooine -> Hoth (6 + 1) -> Endor (1) = 8 days with refueling
        Assert.AreEqual(-1, result.m_numberOfDays);
        Assert.AreEqual(0, result.m_successProbability);

    }

    [TestMethod]
    public void FindShortestPath_TwoOtherValidPlanets_NoBH_ReturnsShortestDistanceWithHundredPercent()
    {
        var repository = CreateTestRepository();
        var bountyHuntersDaysPresence = new BountyHuntersMap();
        var pathfinder = new Pathfinder(repository, bountyHuntersDaysPresence, 6);

        PathData result = pathfinder.FindShortestPath("Dagobah", "Hoth", 3);

        // Dagobah -> Hoth (1) = 1 day
        Assert.AreEqual(1, result.m_numberOfDays);
        Assert.AreEqual(100, result.m_successProbability);
    }

    [TestMethod]
    public void FindShortestPath_FuelCapacityTooSmall_NoBH_ReturnsMinusOneWithHundredPercent()
    {
        var repository = CreateTestRepository();
        var bountyHuntersDaysPresence = new BountyHuntersMap();
        var pathfinder = new Pathfinder(repository, bountyHuntersDaysPresence, 3);

        PathData result = pathfinder.FindShortestPath("Tatooine", "Endor", 15);

        // Both Tatooine neighbors require more fuel than available
        Assert.AreEqual(-1, result.m_numberOfDays);
        Assert.AreEqual(0, result.m_successProbability);
    }

    [TestMethod]
    public void FindShortestPath_ValidPlanetsAndCountdownBigEnough_WithOneForcedBHEncounter_ReturnsShortestDistanceWithProbability()
    {
        var repository = CreateTestRepository();
        var bountyHuntersDaysPresence = new BountyHuntersMap
        {
            { "Hoth", new SortedSet<int> { 6 } }
        };
        var pathfinder = new Pathfinder(repository, bountyHuntersDaysPresence, 6);

        PathData result = pathfinder.FindShortestPath("Tatooine", "Endor", 8);

        // Tatooine -> Hoth (6 + 1) -> Endor (1) = 8 days with refueling
        Assert.AreEqual(8, result.m_numberOfDays);
        Assert.AreEqual(90, result.m_successProbability);
    }

    [TestMethod]
    public void FindShortestPath_ValidPlanetsAndCountdownBigEnoughToDodgeBountyHunters_ReturnsHighestProbability()
    {
        var repository = CreateTestRepository();
        var bountyHuntersDaysPresence = new BountyHuntersMap
        {
            { "Hoth", new SortedSet<int> { 6 } }
        };
        var pathfinder = new Pathfinder(repository, bountyHuntersDaysPresence, 6);

        PathData result = pathfinder.FindShortestPath("Tatooine", "Endor", 10);

        // Tatooine -> Hoth (6 + 1) -> Endor (1) = 8 days with refueling
        Assert.AreEqual(9, result.m_numberOfDays);
        Assert.AreEqual(100, result.m_successProbability);
    }

    [TestMethod]
    public void FindShortestPath_ValidPlanetsAndCountdownBigEnough_WithBHNotCrossed_ReturnsShortestDistanceWithHundredPercent()
    {
        var repository = CreateTestRepository();
        var bountyHuntersDaysPresence = new BountyHuntersMap
        {
            { "Hoth", new SortedSet<int> { 1, 2, 3, 9, 10 } }
        };
        var pathfinder = new Pathfinder(repository, bountyHuntersDaysPresence, 6);

        PathData result = pathfinder.FindShortestPath("Tatooine", "Endor", 10);

        // Tatooine -> Hoth (6 + 1) -> Endor (1) = 8 days with refueling
        Assert.AreEqual(8, result.m_numberOfDays);
        Assert.AreEqual(100, result.m_successProbability);
    }

    [TestMethod]
    public void FindShortestPath_RefuelingOnPlanetWithBountyHunters_ReturnsShortestDistanceWithProbability()
    {
        var repository = CreateTestRepository();
        var bountyHuntersDaysPresence = new BountyHuntersMap
        {
            { "Hoth", new SortedSet<int> { 6, 7, 8 } }
        };
        var pathfinder = new Pathfinder(repository, bountyHuntersDaysPresence, 6);

        PathData result = pathfinder.FindShortestPath("Tatooine", "Endor", 8);

        // Tatooine -> Hoth (6 + 1) -> Endor (1) = 8 days with refueling
        Assert.AreEqual(8, result.m_numberOfDays);
        Assert.AreEqual(81, result.m_successProbability);
    }

    [TestMethod]
    public void FindShortestPath_BountyHuntersArrivingOnRefuelDayAndCountdownToShortToWait_ReturnsHighestProbability()
    {
        var repository = CreateTestRepository();
        var bountyHuntersDaysPresence = new BountyHuntersMap
        {
            { "Hoth", new SortedSet<int> { 7 } },
            { "Dagobah", new SortedSet<int> { 7 } }
        };
        var pathfinder = new Pathfinder(repository, bountyHuntersDaysPresence, 6);

        PathData result = pathfinder.FindShortestPath("Tatooine", "Endor", 9);

        // Tatooine -> Hoth (6 + 1) -> Endor (1) = 8 days with refueling
        Assert.AreEqual(8, result.m_numberOfDays);
        Assert.AreEqual(90, result.m_successProbability);
    }

    [TestMethod]
    public void FindShortestPath_WaitingOneDayDodgesBountyHunters_ReturnsHundredPercentProbability()
    {
        var repository = CreateTestRepository();
        var bountyHuntersDaysPresence = new BountyHuntersMap
        {
            { "Hoth", new SortedSet<int> { 6, 7, 8 } }
        };
        var pathfinder = new Pathfinder(repository, bountyHuntersDaysPresence, 6);

        PathData result = pathfinder.FindShortestPath("Tatooine", "Endor", 10);

        // Tatooine -> Dagobah (6 + 1 + 1) -> Hoth (1) -> Endor (1) = 10 days with refueling + waiting on Dagobah
        Assert.AreEqual(10, result.m_numberOfDays);
        Assert.AreEqual(100, result.m_successProbability);
    }

    [TestMethod]
    public void FindShortestPath_WaitingSeveralDaysDodgesBountyHunters_ReturnsHundredPercentProbability()
    {
        var repository = CreateTestRepository();
        var bountyHuntersDaysPresence = new BountyHuntersMap
        {
            { "Hoth", new SortedSet<int> { 6, 7 } },
            { "Dagobah", new SortedSet<int> { 6, 7, 8 } }
        };
        var pathfinder = new Pathfinder(repository, bountyHuntersDaysPresence, 6);

        PathData result = pathfinder.FindShortestPath("Tatooine", "Endor", 10);

        // Tatooine (2) -> Hoth (6 + 1) -> Endor (1) = 10 days with refueling and waiting
        Assert.AreEqual(10, result.m_numberOfDays);
        Assert.AreEqual(100, result.m_successProbability);
    }

    [TestMethod]
    public void FindShortestPath_ValidPlanetsAndCountdownBigEnoughWithWaitingToDodgeSomeEncounters_ReturnsHighestProbability()
    {
        var repository = CreateTestRepository2();
        var bountyHuntersDaysPresence = new BountyHuntersMap
        {
            { "Hoth", new SortedSet<int> { 3, 4 } },
            { "Dagobah", new SortedSet<int> { 6, 7, 8 } }
        };
        var pathfinder = new Pathfinder(repository, bountyHuntersDaysPresence, 12);

        PathData result = pathfinder.FindShortestPath("Tatooine", "Endor", 10);

        // Tatooine (1) -> Hoth (4) -> Dagobah (3) -> Endor (2) = 10 days with refueling
        Assert.AreEqual(10, result.m_numberOfDays);
        Assert.AreEqual(90, result.m_successProbability);
    }

    [TestMethod]
    public void FindShortestPath_HothToEndor_ReturnsHundredPercentProbability()
    {
        var repository = CreateTestRepository();
        var bountyHuntersDaysPresence = new BountyHuntersMap
        {
            { "Hoth", new SortedSet<int> { 6, 7, 8 } }
        };
        var pathfinder = new Pathfinder(repository, bountyHuntersDaysPresence, 6);

        PathData result = pathfinder.FindShortestPath("Hoth", "Endor", 10);

        // Hoth -> Endor (1) = 1 day with refueling
        Assert.AreEqual(1, result.m_numberOfDays);
        Assert.AreEqual(100, result.m_successProbability);
    }

}
