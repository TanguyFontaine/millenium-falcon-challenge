using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace PathfinderEngine.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PathfinderController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public PathfinderController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Call OnboardComputerService to get Millennium Falcon configuration and universe data
        private async Task<string> GetMillenniumFalconConfigurationAsync()
        {
            string onboardComputerUrl = "http://localhost:5001/api/onboardcomputer/read";
            HttpResponseMessage response = await _httpClient.GetAsync(onboardComputerUrl);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to retrieve Millennium Falcon configuration: {response.StatusCode}");
            }

            return await response.Content.ReadAsStringAsync();
        }

        private MillenniumFalconData RetrieveMillenniumFalconData(string millenniumFalconConfiguration)
        {
            MillenniumFalconDataDto millenniumFalconDataDto = MillenniumFalconParser.Parse(millenniumFalconConfiguration);

            return MillenniumFalconDataFactory.Build(ref millenniumFalconDataDto);
        }

        private EmpireData RetrieveEmpireData(string empireDataJson)
        {
            EmpireDataDto empireDataDto = EmpireDataParser.Parse(empireDataJson);
            return EmpireDataFactory.Build(ref empireDataDto);
        }

        private PathData ComputePath(MillenniumFalconData millenniumFalconData, EmpireData empireData)
        {
            Pathfinder pathfinder = new Pathfinder(millenniumFalconData.m_universe, empireData.m_bountyHuntersPresence, millenniumFalconData.m_autonomy);

            return pathfinder.FindShortestPath(millenniumFalconData.m_departurePlanet, millenniumFalconData.m_arrivalPlanet, empireData.m_countdown);
        }

        [HttpPost("compute")]
        public async Task<IActionResult> ComputePath([FromBody] CalculatePathRequest request)
        {
            try
            {
                string millenniumFalconConfiguration = await GetMillenniumFalconConfigurationAsync();
                MillenniumFalconData millenniumFalconData = RetrieveMillenniumFalconData(millenniumFalconConfiguration);

                EmpireData empireData = RetrieveEmpireData(request.EmpireDataJson);

                PathData pathResult = ComputePath(millenniumFalconData, empireData);

                // Return result
                var response = new
                {
                    numberOfDays = pathResult.m_numberOfDays,
                    successProbability = pathResult.m_successProbability,
                    configuration = new
                    {
                        departure = millenniumFalconData.m_departurePlanet,
                        arrival = millenniumFalconData.m_arrivalPlanet,
                        autonomy = millenniumFalconData.m_autonomy,
                        countdown = empireData.m_countdown
                    }
                };

                return Ok(response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = "Invalid JSON format", message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Internal server error", message = ex.Message });
            }
        }

        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok(new { message = "Empire Data API is running", timestamp = DateTime.UtcNow });
        }
    }

    // Request model for calculate endpoint
    public class CalculatePathRequest
    {
        public string EmpireDataJson { get; set; } = string.Empty;
    }
}
