using Microsoft.Data.SqlClient;
using SportradarCodingExercise.Server.DTOs;
using SportradarCodingExercise.Server.Interfaces;
using SportradarCodingExercise.Server.Models;

namespace SportradarCodingExercise.Server.Services
{
    public class EventRelatedDataService : IEventRelatedDataService
    {
        private readonly string _connectionString;

        public EventRelatedDataService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task<IEnumerable<Competition>> GetCompetitionsAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = "SELECT * FROM Competition";
            var command = new SqlCommand(sql, connection);

            var competitions = new List<Competition>();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                competitions.Add(new Competition
                {
                    CompetitionId = reader.GetInt32(reader.GetOrdinal("CompetitionId")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    CompetitionSlug = reader.GetString(reader.GetOrdinal("CompetitionSlug"))
                });
            }

            return competitions;
        }

        public async Task<IEnumerable<EventDetailDto>> GetEventDetailsByEventIdAsync(int eventId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = @"
                SELECT
                    ed.EventDetailId,
                    ed.RecordedAtUTC,
                    ed.Description,
                    t.TeamId,
                    t.Name AS TeamName,
                    t.OfficialName,
                    t.Slug,
                    t.Abbreviation,
                    c.CountryId,
                    c.Name AS CountryName,
                    c.CountryCode,
                    s.SportId,
                    s.Name AS SportName,
                    et.EventTypeId,
                    et.Name AS EventTypeName
                FROM EventDetail ed
                INNER JOIN Team t ON ed.TeamId = t.TeamId
                INNER JOIN Country c ON t.CountryId = c.CountryId
                INNER JOIN Sport s ON t.SportId = s.SportId
                INNER JOIN EventType et ON ed.EventTypeId = et.EventTypeId
                WHERE ed.EventId = @eventId
                ORDER BY ed.RecordedAtUTC DESC";

            var command = new SqlCommand(sql, connection);

            command.Parameters.AddWithValue("@eventId", eventId);

            var eventDetails = new List<EventDetailDto>();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                eventDetails.Add(new EventDetailDto
                {
                    EventDetailId = reader.GetInt32(reader.GetOrdinal("EventDetailId")),
                    RecordedAtUTC = reader.GetDateTime(reader.GetOrdinal("RecordedAtUTC")),
                    Description = await reader.IsDBNullAsync(reader.GetOrdinal("Description"))
                    ? null : reader.GetString(reader.GetOrdinal("Description")),
                    Team = new Team
                    {
                        TeamId = reader.GetInt32(reader.GetOrdinal("TeamId")),
                        Name = reader.GetString(reader.GetOrdinal("TeamName")),
                        OfficialName = reader.GetString(reader.GetOrdinal("OfficialName")),
                        Slug = reader.GetString(reader.GetOrdinal("Slug")),
                        Abbreviation = reader.GetString(reader.GetOrdinal("Abbreviation")),
                        Country = new Country
                        {
                            CountryId = reader.GetInt32(reader.GetOrdinal("CountryId")),
                            Name = reader.GetString(reader.GetOrdinal("CountryName")),
                            CountryCode = reader.GetString(reader.GetOrdinal("CountryCode"))
                        },
                        Sport = new Sport
                        {
                            SportId = reader.GetInt32(reader.GetOrdinal("SportId")),
                            Name = reader.GetString(reader.GetOrdinal("SportName"))
                        }
                    },
                    EventType = new EventType
                    {
                        EventTypeId = reader.GetInt32(reader.GetOrdinal("EventTypeId")),
                        Name = reader.GetString(reader.GetOrdinal("EventTypeName"))
                    }
                });
            }

            return eventDetails;
        }

        public async Task<IEnumerable<Season>> GetSeasonsAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = "SELECT * FROM Season";
            var command = new SqlCommand(sql, connection);

            var seasons = new List<Season>();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                seasons.Add(new Season
                {
                    SeasonId = reader.GetInt32(reader.GetOrdinal("SeasonId")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    StartDate = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("StartDate"))),
                    EndDate = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("EndDate")))
                });
            }

            return seasons;
        }

        public async Task<IEnumerable<Sport>> GetSportsAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = "SELECT * FROM Sport";
            var command = new SqlCommand(sql, connection);

            var sports = new List<Sport>();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                sports.Add(new Sport
                {
                    SportId = reader.GetInt32(reader.GetOrdinal("SportId")),
                    Name = reader.GetString(reader.GetOrdinal("Name"))
                });
            }

            return sports;
        }

        public async Task<IEnumerable<Stage>> GetStagesAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = "SELECT * FROM Stage";
            var command = new SqlCommand(sql, connection);

            var stages = new List<Stage>();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                stages.Add(new Stage
                {
                    StageId = reader.GetInt32(reader.GetOrdinal("StageId")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    Ordering = reader.GetInt32(reader.GetOrdinal("Ordering"))
                });
            }

            return stages;
        }

        public async Task<IEnumerable<Status>> GetStatusesAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = "SELECT * FROM Status";
            var command = new SqlCommand(sql, connection);

            var statuses = new List<Status>();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                statuses.Add(new Status
                {
                    StatusId = reader.GetInt32(reader.GetOrdinal("StatusId")),
                    Name = reader.GetString(reader.GetOrdinal("Name"))
                });
            }

            return statuses;
        }

        public async Task<IEnumerable<Team>> GetTeamsAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = @"
                SELECT 
                    t.*, 
                    s.SportId, 
                    s.Name AS SportName,
                    c.CountryId,
                    c.Name AS CountryName,
                    c.CountryCode
                FROM Team t
                INNER JOIN Sport s ON t.SportId = s.SportId
                INNER JOIN Country c ON t.CountryId = c.CountryId";

            var command = new SqlCommand(sql, connection);

            var teams = new List<Team>();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                teams.Add(new Team
                {
                    TeamId = reader.GetInt32(reader.GetOrdinal("TeamId")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    OfficialName = reader.GetString(reader.GetOrdinal("OfficialName")),
                    Slug = reader.GetString(reader.GetOrdinal("Slug")),
                    Abbreviation = reader.GetString(reader.GetOrdinal("Abbreviation")),
                    Sport = new Sport
                    {
                        SportId = reader.GetInt32(reader.GetOrdinal("SportId")),
                        Name = reader.GetString(reader.GetOrdinal("SportName"))
                    },
                    Country = new Country
                    {
                        CountryId = reader.GetInt32(reader.GetOrdinal("CountryId")),
                        Name = reader.GetString(reader.GetOrdinal("CountryName")),
                        CountryCode = reader.GetString(reader.GetOrdinal("CountryCode"))
                    }
                });
            }

            return teams;
        }

        public async Task<IEnumerable<Venue>> GetVenuesAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = @"
                SELECT
                    v.*,
                    a.AddressId,
                    a.StreetNumber,
                    a.StreetName,
                    a.City,
                    c.CountryId,
                    c.Name AS CountryName,
                    c.CountryCode
                FROM Venue v
                INNER JOIN Address a ON v.AddressId = a.AddressId
                INNER JOIN Country c ON a.CountryId = c.CountryId";

            var command = new SqlCommand(sql, connection);

            var venues = new List<Venue>();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                venues.Add(new Venue
                {
                    VenueId = reader.GetInt32(reader.GetOrdinal("VenueId")),
                    Name = reader.GetString(reader.GetOrdinal("Name")),
                    Capacity = reader.GetInt32(reader.GetOrdinal("Capacity")),
                    Address = new Address
                    {
                        AddressId = reader.GetInt32(reader.GetOrdinal("AddressId")),
                        StreetNumber = reader.GetString(reader.GetOrdinal("StreetNumber")),
                        StreetName = reader.GetString(reader.GetOrdinal("StreetName")),
                        City = reader.GetString(reader.GetOrdinal("City")),
                        Country = new Country
                        {
                            CountryId = reader.GetInt32(reader.GetOrdinal("CountryId")),
                            Name = reader.GetString(reader.GetOrdinal("CountryName")),
                            CountryCode = reader.GetString(reader.GetOrdinal("CountryCode"))
                        }
                    }
                });
            }

            return venues;
        }
    }
}
