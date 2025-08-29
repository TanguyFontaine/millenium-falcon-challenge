using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace PathfinderEngine.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PathfinderController : ControllerBase
    {
        public PathfinderController()
        {}

        [HttpPost("compute")]
        public IActionResult ComputePath([FromBody] CalculatePathRequest request)
        {
            try
            {
                // dummy millenium falcon data
                int millenniumFalconAutonomy = 6;
                string departurePlanet = "Tatooine";
                string arrivalPlanet = "Endor";
                var universeRepository = CreateMockUniverseRepository();

                // Retrieve empire data
                EmpireDataDto empireDataDTO = EmpireDataParser.Parse(request.EmpireDataJson);
                EmpireData empireData = EmpireDataFactory.Build(ref empireDataDTO);
                var bountyHuntersMap = empireData.m_bountyHuntersPresence;

                Pathfinder pathfinder = new Pathfinder(ref universeRepository, ref bountyHuntersMap, millenniumFalconAutonomy);

                PathData pathResult = pathfinder.FindShortestPath(departurePlanet, arrivalPlanet, empireData.m_countdown);

                // Return result
                var response = new
                {
                    numberOfDays = pathResult.m_numberOfDays,
                    successProbability = pathResult.m_successProbability,
                    configuration = new
                    {
                        departure = departurePlanet,
                        arrival = arrivalPlanet,
                        autonomy = millenniumFalconAutonomy,
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

        private UniverseGraphRepository CreateMockUniverseRepository()
        {
            var repository = new UniverseGraphRepository();
            
            var tatooine = new Planet("Tatooine");
            var dagobah = new Planet("Dagobah");
            var endor = new Planet("Endor");
            var hoth = new Planet("Hoth");
            
            tatooine.AddNeighbor(dagobah, 6);
            tatooine.AddNeighbor(hoth, 6);
            dagobah.AddNeighbor(endor, 4);
            dagobah.AddNeighbor(hoth, 1);
            hoth.AddNeighbor(endor, 1);
            hoth.AddNeighbor(dagobah, 1);
            endor.AddNeighbor(dagobah, 4);
            endor.AddNeighbor(hoth, 1);

            repository.AddPlanet(tatooine);
            repository.AddPlanet(dagobah);
            repository.AddPlanet(endor);
            repository.AddPlanet(hoth);
            
            return repository;
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
