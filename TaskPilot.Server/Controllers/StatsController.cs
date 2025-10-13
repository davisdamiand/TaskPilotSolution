using Microsoft.AspNetCore.Mvc;
using TaskPilot.Server.Interfaces;
using Shared.DTOs;

namespace TaskPilot.Server.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class StatsController : Controller
    {
        private readonly IStatsService _statsService;

        public StatsController(IStatsService statsService)
        {
            _statsService = statsService;
        }

        [HttpGet]
        [Route("CreateStats")]

        public async Task<IActionResult> CreateStats([FromBody] StatsCreateDto statsCreateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                     .SelectMany(x => x.Value.Errors)
                     .Select(x => x.ErrorMessage)
                     .ToList();

                    Console.WriteLine(new {errors =  errors});
                    return BadRequest("Technical difficulties");
                }

                var id = await _statsService.CreateStatsAsync(statsCreateDto);

                if (id < 0)
                {
                    return BadRequest("Stats creation failed - Stats was not generated");
                }

                return Ok(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest("Stats creation failed - Stats was not generated");
                throw;
            }
        }

        [HttpPost]
        [Route("GetStats")]
        public async Task<IActionResult> GetStats([FromBody] StatsCalculateDto statsCalculateDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                     .SelectMany(x => x.Value.Errors)
                     .Select(x => x.ErrorMessage)
                     .ToList();

                    Console.WriteLine(new { errors = errors });
                    return BadRequest("Technical difficulties");
                }

                var statsSendDto = await _statsService.SendStatsAsync(statsCalculateDto);

                if (statsSendDto == null)
                {
                    return BadRequest("Failed to get stats from the Database");
                }

                return Ok(statsSendDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest("Failed to get stats from the Database");
                throw;
            }
        }
    }
}
