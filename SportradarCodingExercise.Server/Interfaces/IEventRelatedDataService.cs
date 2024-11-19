using SportradarCodingExercise.Server.DTOs;
using SportradarCodingExercise.Server.Models;

namespace SportradarCodingExercise.Server.Interfaces
{
    public interface IEventRelatedDataService
    {
        Task<IEnumerable<Status>> GetStatusesAsync();
        Task<IEnumerable<Team>> GetTeamsAsync();
        Task<IEnumerable<Venue>> GetVenuesAsync();
        Task<IEnumerable<Stage>> GetStagesAsync();
        Task<IEnumerable<Competition>> GetCompetitionsAsync();
        Task<IEnumerable<Sport>> GetSportsAsync();
        Task<IEnumerable<Season>> GetSeasonsAsync();
        Task<IEnumerable<EventDetailDto>> GetEventDetailsByEventIdAsync(int eventId);

    }
}
