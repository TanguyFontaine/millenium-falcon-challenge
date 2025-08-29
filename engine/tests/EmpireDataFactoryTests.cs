using Microsoft.VisualStudio.TestTools.UnitTesting;

using BountyHuntersMap = System.Collections.Generic.Dictionary<string, System.Collections.Generic.SortedSet<int>>;

[TestClass]
public class EmpireDataFactoryTests
{
    [TestMethod]
    public void BuildFrom_ValidSimpleDto()
    {
        var dto = new EmpireDataDto
        {
            m_countdown = 7,
            m_bountyHunters = new List<BountyHunterDto>
                {
                    new BountyHunterDto { m_planet = "Hoth", m_day = 6 },
                    new BountyHunterDto { m_planet = "Hoth", m_day = 7 },
                    new BountyHunterDto { m_planet = "Hoth", m_day = 8 }
                }
        };

        EmpireData actualData = EmpireDataFactory.Build(ref dto);

        EmpireData expectedData = new EmpireData
        {
            m_countdown = 7,
            m_bountyHuntersPresence = new BountyHuntersMap
            {
                { "Hoth", new SortedSet<int> { 6, 7, 8 } }
            }
        };

        Assert.AreEqual(expectedData, actualData);
    }

    [TestMethod]
    public void Build_MultiplePlanets_GroupsCorrectlyInOrder()
    {
        var dto = new EmpireDataDto
        {
            m_countdown = 10,
            m_bountyHunters = new List<BountyHunterDto>
                {
                    new BountyHunterDto { m_planet = "Hoth", m_day = 6 },
                    new BountyHunterDto { m_planet = "Tatooine", m_day = 4 },
                    new BountyHunterDto { m_planet = "Hoth", m_day = 7 },
                    new BountyHunterDto { m_planet = "Tatooine", m_day = 4 },
                    new BountyHunterDto { m_planet = "Dagobah", m_day = 1 }
                }
        };

        EmpireData actualData = EmpireDataFactory.Build(ref dto);

        EmpireData expectedData = new EmpireData
        {
            m_countdown = 10,
            m_bountyHuntersPresence = new BountyHuntersMap
            {
                { "Dagobah", new SortedSet<int> { 1 } },
                { "Hoth", new SortedSet<int> { 6, 7 } },
                { "Tatooine", new SortedSet<int> { 4 } }
            }
        };

        Assert.AreEqual(expectedData, actualData);
    }

    [TestMethod]
    public void Build_DuplicateDaysOnSamePlanet()
    {
        var dto = new EmpireDataDto
        {
            m_countdown = 5,
            m_bountyHunters = new List<BountyHunterDto>
                {
                    new BountyHunterDto { m_planet = "Endor", m_day = 3 },
                    new BountyHunterDto { m_planet = "Endor", m_day = 3 },
                    new BountyHunterDto { m_planet = "Endor", m_day = 5 },
                    new BountyHunterDto { m_planet = "Endor", m_day = 3 },
                    new BountyHunterDto { m_planet = "Endor", m_day = 1 }
                }
        };

        EmpireData actualData = EmpireDataFactory.Build(ref dto);

        EmpireData expectedData = new EmpireData
        {
            m_countdown = 5,
            m_bountyHuntersPresence = new BountyHuntersMap
            {
                { "Endor", new SortedSet<int> { 1, 3, 5 } }
            }
        };

        Assert.AreEqual(expectedData, actualData);
    }

    [TestMethod]
    public void Build_EmptyBountyHunters_ReturnsEmptyMap()
    {
        var dto = new EmpireDataDto
        {
            m_countdown = 15,
            m_bountyHunters = new List<BountyHunterDto>()
        };

        EmpireData actualData = EmpireDataFactory.Build(ref dto);

        EmpireData expectedData = new EmpireData
        {
            m_countdown = 15,
            m_bountyHuntersPresence = new BountyHuntersMap()
        };

        Assert.AreEqual(expectedData, actualData);
    }

    [TestMethod]
    public void Build_EmptyPlanetNamesAreSkipped()
    {
        var dto = new EmpireDataDto
        {
            m_countdown = 8,
            m_bountyHunters = new List<BountyHunterDto>
                {
                    new BountyHunterDto { m_planet = "Coruscant", m_day = 2 },
                    new BountyHunterDto { m_planet = "", m_day = 3 },
                    new BountyHunterDto { m_planet = "Naboo", m_day = 4 }
                }
        };

        EmpireData actualData = EmpireDataFactory.Build(ref dto);

        EmpireData expectedData = new EmpireData
        {
            m_countdown = 8,
            m_bountyHuntersPresence = new BountyHuntersMap
            {
                { "Coruscant", new SortedSet<int> { 2 } },
                { "Naboo", new SortedSet<int> { 4 } }
            }
        };

        Assert.AreEqual(expectedData, actualData);
    }

    [TestMethod]
    public void Build_NullBountyHuntersListIsHandled()
    {
        var dto = new EmpireDataDto
        {
            m_countdown = 42,
            m_bountyHunters = null!
        };

        EmpireData actualData = EmpireDataFactory.Build(ref dto);

        EmpireData expectedData = new EmpireData
        {
            m_countdown = 42,
            m_bountyHuntersPresence = new BountyHuntersMap()
        };

        Assert.AreEqual(expectedData, actualData);
    }
}
