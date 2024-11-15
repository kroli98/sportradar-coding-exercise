using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SportradarCodingExercise.Server.Interfaces;
using SportradarCodingExercise.Server.Models;
using System.Globalization;

namespace SportradarCodingExercise.Server.Controllers
{
    [Route("api/sport-events")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Event>>> GetEvents()
        {
            try
            {
                var events = await _eventService.GetEventsAsync();

                if (!events.Any())
                {
                    return NoContent();
                }

                return Ok(events);
            }
            catch(Exception)
            {
                return StatusCode(500);
            }
        }

        [HttpGet("/id/{id}")]
        public async Task<ActionResult<Event>> GetEvent(int id)
        {
            try
            {
                var evnt = await _eventService.GetEventByIdAsync(id);

                if (evnt == null)
                {
                    return NotFound();
                }

                return Ok(evnt);
            }
            catch(Exception)
            {
                return StatusCode(500);
            }
        }

        [HttpGet("/date/{date}")]
        public async Task<ActionResult<IEnumerable<Event>>> GetEventsByDate(string date)
        {
            try
            {
                if (!DateOnly.TryParse(date, CultureInfo.InvariantCulture, out DateOnly parsedDate))
                {
                    return BadRequest("Invalid date format. Use YYYY-MM-DD");
                }

                var events = await _eventService.GetEventsByDateAsync(parsedDate);

                if (!events.Any())
                {
                    return NoContent();
                }

                return Ok(events);
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
    }
}
