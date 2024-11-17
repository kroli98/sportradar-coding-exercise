using SportradarCodingExercise.Server.DTOs;
using SportradarCodingExercise.Server.Models;

namespace SportradarCodingExercise.Server.Interfaces
{
    /// <summary>
    /// Defines the contract for event management operations.
    /// </summary>
    public interface IEventService
    {
        /// <summary>
        /// Gets all events with their related data
        /// Excludes event related history (EventDetails)
        /// </summary>
        /// <returns>Collection of events.</returns>
        Task<IEnumerable<Event>> GetEventsAsync();

        /// <summary>
        /// Gets events for a specific date.
        /// Excludes event related history (EventDetails)
        /// </summary>
        /// <param name="date">Date to filter by.</param>
        /// <returns>Collection of events on the specified date.</returns>
        Task<IEnumerable<Event>> GetEventsByDateAsync(DateOnly date);

        /// <summary>
        /// Gets events for a specific sport.
        /// Excludes event related history (EventDetails)
        /// </summary>
        /// <param name="sportId">Sport ID to filter by.</param>
        /// <returns>Collection of events for the specified sport.</returns>
        Task<IEnumerable<Event>> GetEventsBySportIdAsync(int sportId);

        /// <summary>
        /// Gets a specific event by its unique identifier.
        /// Excludes event related history (EventDetails)
        /// </summary>
        /// <param name="id">The event ID to look up.</param>
        /// <returns>The requested event or null if not found.</returns>
        Task<Event?> GetEventByIdAsync(int id);

        /// <summary>
        /// Adds a new event to the database.
        /// </summary>
        /// <param name="evnt">The event to add.</param>
        /// <returns>The created event with its assigned ID.</returns>
        Task<Event> AddEventAsync(CreateEventDto evnt);
    }
}
