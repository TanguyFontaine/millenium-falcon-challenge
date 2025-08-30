using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class MillenniumFalconDataParserTests
{
    [TestMethod]
    public void Parse_ValidSimpleJson_ParsesCorrectly()
    {
        string jsonString = @"{
                ""autonomy"": 6,
                ""departure"": ""Tatooine"",
                ""arrival"": ""Endor"",
                ""routes_data"": [
                    {""origin"": ""Tatooine"", ""destination"": ""Dagobah"", ""distance"": 6},
                    {""origin"": ""Dagobah"", ""destination"": ""Endor"", ""distance"": 4},
                    {""origin"": ""Dagobah"", ""destination"": ""Hoth"", ""distance"": 1}
                ]
            }";

        MillenniumFalconDataDto actual = MillenniumFalconParser.Parse(jsonString);

        MillenniumFalconDataDto expected = new MillenniumFalconDataDto
        {
            m_autonomy = 6,
            m_departure = "Tatooine",
            m_arrival = "Endor",
            m_routes = new List<RouteDto>
            {
                new RouteDto { m_origin = "Tatooine", m_destination = "Dagobah", m_distance = 6 },
                new RouteDto { m_origin = "Dagobah", m_destination = "Endor", m_distance = 4 },
                new RouteDto { m_origin = "Dagobah", m_destination = "Hoth", m_distance = 1 }
            }
        };

        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Parse_MultipleRoutes_ParsesCorrectly()
    {
        string jsonString = @"{
                ""autonomy"": 12,
                ""departure"": ""Alderaan"",
                ""arrival"": ""Coruscant"",
                ""routes_data"": [
                    {""origin"": ""Alderaan"", ""destination"": ""Naboo"", ""distance"": 3},
                    {""origin"": ""Naboo"", ""destination"": ""Coruscant"", ""distance"": 2},
                    {""origin"": ""Alderaan"", ""destination"": ""Kamino"", ""distance"": 5},
                    {""origin"": ""Kamino"", ""destination"": ""Coruscant"", ""distance"": 4},
                    {""origin"": ""Naboo"", ""destination"": ""Kamino"", ""distance"": 1}
                ]
            }";

        MillenniumFalconDataDto actual = MillenniumFalconParser.Parse(jsonString);

        MillenniumFalconDataDto expected = new MillenniumFalconDataDto
        {
            m_autonomy = 12,
            m_departure = "Alderaan",
            m_arrival = "Coruscant",
            m_routes = new List<RouteDto>
            {
                new RouteDto { m_origin = "Alderaan", m_destination = "Naboo", m_distance = 3 },
                new RouteDto { m_origin = "Naboo", m_destination = "Coruscant", m_distance = 2 },
                new RouteDto { m_origin = "Alderaan", m_destination = "Kamino", m_distance = 5 },
                new RouteDto { m_origin = "Kamino", m_destination = "Coruscant", m_distance = 4 },
                new RouteDto { m_origin = "Naboo", m_destination = "Kamino", m_distance = 1 }
            }
        };

        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Parse_EmptyRoutes_ParsesCorrectly()
    {
        string jsonString = @"{
                ""autonomy"": 8,
                ""departure"": ""Tatooine"",
                ""arrival"": ""Endor"",
                ""routes_data"": []
            }";

        MillenniumFalconDataDto actual = MillenniumFalconParser.Parse(jsonString);

        MillenniumFalconDataDto expected = new MillenniumFalconDataDto
        {
            m_autonomy = 8,
            m_departure = "Tatooine",
            m_arrival = "Endor",
            m_routes = new List<RouteDto>()
        };

        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void Parse_InvalidJson_ThrowsException()
    {
         // Missing comma after arrival
        string invalidJson = @"{
                ""autonomy"": 6,
                ""departure"": ""Tatooine"",
                ""arrival"": ""Endor""
                ""routes_data"": [
                    {""origin"": ""Tatooine"", ""destination"": ""Endor"", ""distance"": 1}
                ]
            }";

        Assert.ThrowsException<ArgumentException>(() => MillenniumFalconParser.Parse(invalidJson));
    }

    [TestMethod]
    public void Parse_NullString_ThrowsException()
    {
        string nullJson = null!;

        Assert.ThrowsException<ArgumentNullException>(() => MillenniumFalconParser.Parse(nullJson));
    }

    [TestMethod]
    public void Parse_MissingRequiredFields_ParsesWithDefaults()
    {
        string jsonString = @"{
                ""autonomy"": 6
            }";

        MillenniumFalconDataDto actual = MillenniumFalconParser.Parse(jsonString);

        MillenniumFalconDataDto expected = new MillenniumFalconDataDto
        {
            m_autonomy = 6,
            m_departure = string.Empty,
            m_arrival = string.Empty,
            m_routes = new List<RouteDto>()
        };

        Assert.AreEqual(expected, actual);
    }
}
