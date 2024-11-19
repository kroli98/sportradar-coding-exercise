using Microsoft.AspNetCore.Mvc;
using SportradarCodingExercise.Server.Interfaces;

namespace SportradarCodingExercise.Server.Controllers
{
    [ApiController]
    [Route("api/")]
    public class EventRelatedDataController : ControllerBase
    {
        private readonly IEventRelatedDataService _eventRelatedDataService;

        public EventRelatedDataController(IEventRelatedDataService eventRelatedDataService)
        {
            _eventRelatedDataService = eventRelatedDataService;
        }

        [HttpGet("competitions")]
        public async Task<IActionResult> GetCompetitions()
        {
            try
            {
                var competitions = await _eventRelatedDataService.GetCompetitionsAsync();
                return Ok(competitions);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        [HttpGet("seasons")]
        public async Task<IActionResult> GetSeasons()
        {
            try
            {
                var seasons = await _eventRelatedDataService.GetSeasonsAsync();
                return Ok(seasons);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        [HttpGet("sports")]
        public async Task<IActionResult> GetSports()
        {
            try
            {
                var sports = await _eventRelatedDataService.GetSportsAsync();
                return Ok(sports);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        [HttpGet("stages")]
        public async Task<IActionResult> GetStages()
        {
            try
            {
                var stages = await _eventRelatedDataService.GetStagesAsync();
                return Ok(stages);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        [HttpGet("statuses")]
        public async Task<IActionResult> GetStatuses()
        {
            try
            {
                var statuses = await _eventRelatedDataService.GetStatusesAsync();
                return Ok(statuses);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        [HttpGet("teams")]
        public async Task<IActionResult> GetTeams()
        {
            try
            {
                var teams = await _eventRelatedDataService.GetTeamsAsync();
                return Ok(teams);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        [HttpGet("venues")]
        public async Task<IActionResult> GetVenues()
        {
            try
            {
                var venues = await _eventRelatedDataService.GetVenuesAsync();
                return Ok(venues);
            }
            catch
            {
                return StatusCode(500);
            }
        }
    }
}