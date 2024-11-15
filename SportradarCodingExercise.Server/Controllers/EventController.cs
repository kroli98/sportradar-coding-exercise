using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SportradarCodingExercise.Server.Interfaces;
using SportradarCodingExercise.Server.Models;

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
            var events = await _eventService.GetEventsAsync();

            if(!events.Any())
            {
                return NoContent();
            }

            return Ok(events);
        }
    }
}
