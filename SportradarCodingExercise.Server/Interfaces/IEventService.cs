using SportradarCodingExercise.Server.Models;

namespace SportradarCodingExercise.Server.Interfaces
{
    public interface IEventService
    {
        Task<IEnumerable<Event>> GetEventsAsync();
        Task<IEnumerable<Event>> GetEventsByDateAsync(DateOnly date);
        Task<IEnumerable<Event>> GetEventsBySportIdAsync(int sportId);
        Task<Event> GetEventByIdAsync(int id);
        Task<Event> AddEventAsync(Event evnt);
    }
}
