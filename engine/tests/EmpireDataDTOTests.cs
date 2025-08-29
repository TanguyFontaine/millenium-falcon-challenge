using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class EmpireDataDTOTests
{
    [TestMethod]
    public void Parse_ValidSimpleJson_ConvertsCorrectly()
    {
        string jsonString = @"{
                ""countdown"": 7,
                ""bounty_hunters"": [
                    {""planet"": ""Hoth"", ""day"": 6},
                    {""planet"": ""Hoth"", ""day"": 7},
                    {""planet"": ""Hoth"", ""day"": 8}
                ]
            }";

        EmpireDataDto actual = EmpireDataParser.Parse(jsonString);

        EmpireDataDto expected = new EmpireDataDto
        {
            m_countdown = 7,
            m_bountyHunters = new List<BountyHunterDto>
            {
                new BountyHunterDto { m_planet = "Hoth", m_day = 6 },
                new BountyHunterDto { m_planet = "Hoth", m_day = 7 },
                new BountyHunterDto { m_planet = "Hoth", m_day = 8 }
            }
        };

        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Parse_MultiplePlanets_ParsesCorrectly()
    {
        string jsonString = @"{
                ""countdown"": 10,
                ""bounty_hunters"": [
                    {""planet"": ""Hoth"", ""day"": 6},
                    {""planet"": ""Tatooine"", ""day"": 4},
                    {""planet"": ""Hoth"", ""day"": 7},
                    {""planet"": ""Tatooine"", ""day"": 4},
                    {""planet"": ""Dagobah"", ""day"": 1}
                ]
            }";

        EmpireDataDto actual = EmpireDataParser.Parse(jsonString);

        EmpireDataDto expected = new EmpireDataDto
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

        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Parse_EmptyBountyHunters_ReturnsEmptyList()
    {
        string jsonString = @"{
                ""countdown"": 15,
                ""bounty_hunters"": []
            }";

        EmpireDataDto actual = EmpireDataParser.Parse(jsonString);

        EmpireDataDto expected = new EmpireDataDto
        {
            m_countdown = 15,
            m_bountyHunters = new List<BountyHunterDto>()
        };

        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Parse_EmptyPlanetNames_ParsedAsEmpty()
    {
        string jsonString = @"{
                ""countdown"": 8,
                ""bounty_hunters"": [
                    {""planet"": ""Coruscant"", ""day"": 2},
                    {""planet"": """", ""day"": 3},
                    {""planet"": ""Naboo"", ""day"": 4}
                ]
            }";

        EmpireDataDto actual = EmpireDataParser.Parse(jsonString);

        EmpireDataDto expected = new EmpireDataDto
        {
            m_countdown = 8,
            m_bountyHunters = new List<BountyHunterDto>
            {
                new BountyHunterDto { m_planet = "Coruscant", m_day = 2 },
                new BountyHunterDto { m_planet = "", m_day = 3 },
                new BountyHunterDto { m_planet = "Naboo", m_day = 4 }
            }
        };

        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Parse_InvalidJson_ThrowsArgumentException()
    {
        // Arrange
        string invalidJson = @"{ ""countdown"": 5, ""bounty_hunters"": [ invalid json }";

        // Act - Should throw during parsing
        EmpireDataParser.Parse(invalidJson);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidDataException))]
    public void Parse_NullResult_ThrowsInvalidDataException()
    {
        // Arrange - JSON that deserializes to null
        string nullJson = "null";

        // Act - Should throw InvalidDataException
        EmpireDataParser.Parse(nullJson);
    }

    [TestMethod]
    public void Parse_ZeroCountdown_HandlesCorrectly()
    {
        string jsonString = @"{
                ""countdown"": 0,
                ""bounty_hunters"": [
                    {""planet"": ""DeathStar"", ""day"": 0}
                ]
            }";

        EmpireDataDto actual = EmpireDataParser.Parse(jsonString);

        EmpireDataDto expected = new EmpireDataDto
        {
            m_countdown = 0,
            m_bountyHunters = new List<BountyHunterDto>
            {
                new BountyHunterDto { m_planet = "DeathStar", m_day = 0 }
            }
        };
    }


    [TestMethod]
    public void Parse_InsensitiveCase_IsCorrectlyHandled()
    {
        string jsonString = @"{
                ""COUNTDOWN"": 12,
                ""BOUNTY_HUNTERS"": [
                    {""PLANET"": ""Alderaan"", ""DAY"": 3}
                ]
            }";

        EmpireDataDto actual = EmpireDataParser.Parse(jsonString);

        EmpireDataDto expected = new EmpireDataDto
        {
            m_countdown = 12,
            m_bountyHunters = new List<BountyHunterDto>
            {
                new BountyHunterDto { m_planet = "Alderaan", m_day = 3 }
            }
        };

        Assert.AreEqual(expected, actual);
    }
}