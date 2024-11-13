using SportradarCodingExercise.Server.Interfaces;
using SportradarCodingExercise.Server.Models;

namespace SportradarCodingExercise.Server.Services
{
    public class EventService : IEventService
    {
        public Task<Event> AddEventAsync(Event evnt)
        {
            throw new NotImplementedException();
        }

        public Task<Event> GetEventByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Event>> GetEventsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Event>> GetEventsByDateAsync(DateOnly date)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Event>> GetEventsBySportIdAsync(int sportId)
        {
            throw new NotImplementedException();
        }
    }
}
