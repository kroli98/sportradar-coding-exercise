using Microsoft.Data.SqlClient;
using SportradarCodingExercise.Server.Interfaces;
using SportradarCodingExercise.Server.Models;

namespace SportradarCodingExercise.Server.Services
{
    /// <summary>
    /// Service for managing sport events .
    /// </summary>
    public class EventService : IEventService
    {
        private readonly string _connectionString;

        /// <summary>
        /// Initializes a new instance of the EventService.
        /// </summary>
        /// <param name="configuration">Application configuration containing connection strings.</param>
        /// <exception cref="ArgumentNullException">Thrown when the connection string "DefaultConnection" is not found in the configuration.</exception>
        public EventService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new ArgumentNullException(nameof(configuration));

        }

        /// <inheritdoc/>
        public Task<Event> AddEventAsync(Event evnt)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public async Task<Event?> GetEventByIdAsync(int id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = GetEventsQuery() + " WHERE e.EventId = @EventId";
            var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@EventId", id);

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                return MapEventFromReader(reader);
            }

            return null;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Event>> GetEventsAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = GetEventsQuery();
            var command = new SqlCommand(sql, connection);

            var events = new List<Event>();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                events.Add(MapEventFromReader(reader));
            }

            return events;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Event>> GetEventsByDateAsync(DateOnly date)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var sql = GetEventsQuery() + " WHERE e.Date = @Date ORDER BY e.TimeUTC";

            var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Date", date.ToDateTime(TimeOnly.MinValue));

            var events = new List<Event>();
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                events.Add(MapEventFromReader(reader));
            }

            return events;
        }

        /// <inheritdoc/>
        public Task<IEnumerable<Event>> GetEventsBySportIdAsync(int sportId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Constructs the SQL query for getting events.
        /// </summary>
        /// <returns>SQL query string for getting all events.</returns>
        private static string GetEventsQuery()
        {
            return @"
               SELECT 
                   -- Event
                   e.EventId, 
                   e.Date, 
                   e.TimeUTC, 
                   e.DurationInMinutes, 
                   e.Description, 
                   e.HomeScore, 
                   e.AwayScore,
                   -- Status
                   s.StatusId,
                   s.Name as StatusName,
                   -- Home Team
                   ht.TeamId as HomeTeamId,
                   ht.Name as HomeTeamName,
                   ht.OfficialName as HomeTeamOfficialName,
                   ht.Slug as HomeTeamSlug,
                   ht.Abbreviation as HomeTeamAbbreviation,
                   ht.CountryId as HomeTeamCountryId,
                   -- HomeTeam Sport
                   hts.SportId as HomeTeamSportId,
                   hts.Name as HomeTeamSportName,
                   -- HomeTeam Country
                   hc.Name as HomeTeamCountryName,
                   hc.CountryCode as HomeTeamCountryCode,
                   -- Away Team
                   at.TeamId as AwayTeamId,
                   at.Name as AwayTeamName,
                   at.OfficialName as AwayTeamOfficialName,
                   at.Slug as AwayTeamSlug,
                   at.Abbreviation as AwayTeamAbbreviation,
                   at.CountryId as AwayTeamCountryId,
                   -- AwayTeam Sport
                   ats.SportId as AwayTeamSportId,
                   ats.Name as AwayTeamSportName,
                   -- AwayTeam Country
                   ac.Name as AwayTeamCountryName,
                   ac.CountryCode as AwayTeamCountryCode,
                   -- Venue
                   v.VenueId,
                   v.Name as VenueName,
                   v.Capacity as VenueCapacity,
                   -- Venue Address
                   a.AddressId as VenueAddressId,
                   a.StreetNumber as VenueStreetNumber,
                   a.StreetName as VenueStreet,
                   a.City as VenueCity,
                   --Venue Country
                   vc.CountryId as VenueCountryId,
                   vc.Name as VenueCountryName,
                   vc.CountryCode as VenueCountryCode,
                   -- Stage
                   st.StageId,
                   st.Name as StageName,
                   st.Ordering as StageOrdering,
                   -- Competition
                   c.CompetitionId,
                   c.Name as CompetitionName,
                   c.CompetitionSlug,
                   -- Season (Optional)
                   se.SeasonId,
                   se.Name as SeasonName,
                   se.StartDate as SeasonStartDate,
                   se.EndDate as SeasonEndDate,
                   -- Sport
                   sp.SportId,
                   sp.Name as SportName
               FROM Event e
               INNER JOIN Status s ON e.StatusId = s.StatusId
               INNER JOIN Team ht ON e.HomeTeamId = ht.TeamId
               INNER JOIN Sport hts ON ht.SportId = hts.SportId
               INNER JOIN Country hc ON ht.CountryId = hc.CountryId
               INNER JOIN Team at ON e.AwayTeamId = at.TeamId
               INNER JOIN Sport ats ON at.SportId = ats.SportId
               INNER JOIN Country ac ON at.CountryId = ac.CountryId
               INNER JOIN Venue v ON e.VenueId = v.VenueId
               INNER JOIN Address a ON v.AddressId = a.AddressId
               INNER JOIN Country vc ON a.CountryId = vc.CountryId
               INNER JOIN Stage st ON e.StageId = st.StageId
               INNER JOIN Competition c ON e.CompetitionId = c.CompetitionId
               LEFT JOIN Season se ON c.SeasonId = se.SeasonId
               INNER JOIN Sport sp ON e.SportId = sp.SportId";
        }

        /// <summary>
        /// Maps database reader to Event model and all its nested models.
        /// </summary>
        /// <param name="reader">SQL data reader for event record.</param>
        /// <returns>Populated Event model.</returns>
        private Event MapEventFromReader(SqlDataReader reader)
        {
            return new Event
            {
                EventId = reader.GetInt32(reader.GetOrdinal("EventId")),
                Date = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("Date"))),
                TimeUTC = TimeOnly.FromTimeSpan(reader.GetTimeSpan(reader.GetOrdinal("TimeUTC"))),
                DurationInMinutes = reader.GetInt32(reader.GetOrdinal("DurationInMinutes")),
                Description = reader.IsDBNull(reader.GetOrdinal("Description"))
                ? null: reader.GetString(reader.GetOrdinal("Description")),
                HomeScore = reader.GetInt32(reader.GetOrdinal("HomeScore")),
                AwayScore = reader.GetInt32(reader.GetOrdinal("AwayScore")),

                HomeTeam = new Team
                {
                    TeamId = reader.GetInt32(reader.GetOrdinal("HomeTeamId")),
                    Name = reader.GetString(reader.GetOrdinal("HomeTeamName")),
                    OfficialName = reader.GetString(reader.GetOrdinal("HomeTeamOfficialName")),
                    Slug = reader.GetString(reader.GetOrdinal("HomeTeamSlug")),
                    Abbreviation = reader.GetString(reader.GetOrdinal("HomeTeamAbbreviation")),
                    Country = new Country
                    {
                        CountryId = reader.GetInt32(reader.GetOrdinal("HomeTeamCountryId")),
                        Name = reader.GetString(reader.GetOrdinal("HomeTeamCountryName")),
                        CountryCode = reader.GetString(reader.GetOrdinal("HomeTeamCountryCode"))
                    },
                    Sport = new Sport
                    {
                        SportId = reader.GetInt32(reader.GetOrdinal("HomeTeamSportId")),
                        Name = reader.GetString(reader.GetOrdinal("HomeTeamSportName"))
                    }
                },
                AwayTeam = new Team
                {
                    TeamId = reader.GetInt32(reader.GetOrdinal("AwayTeamId")),
                    Name = reader.GetString(reader.GetOrdinal("AwayTeamName")),
                    OfficialName = reader.GetString(reader.GetOrdinal("AwayTeamOfficialName")),
                    Slug = reader.GetString(reader.GetOrdinal("AwayTeamSlug")),
                    Abbreviation = reader.GetString(reader.GetOrdinal("AwayTeamAbbreviation")),
                    Country = new Country
                    {
                        CountryId = reader.GetInt32(reader.GetOrdinal("AwayTeamCountryId")),
                        Name = reader.GetString(reader.GetOrdinal("AwayTeamCountryName")),
                        CountryCode = reader.GetString(reader.GetOrdinal("AwayTeamCountryCode"))
                    },
                    Sport = new Sport
                    {
                        SportId = reader.GetInt32(reader.GetOrdinal("AwayTeamSportId")),
                        Name = reader.GetString(reader.GetOrdinal("AwayTeamSportName"))
                    }
                },
                Status = new Status
                {
                    StatusId = reader.GetInt32(reader.GetOrdinal("StatusId")),
                    Name = reader.GetString(reader.GetOrdinal("StatusName"))
                },
                Competition = new Competition
                {
                    CompetitionId = reader.GetInt32(reader.GetOrdinal("CompetitionId")),
                    Name = reader.GetString(reader.GetOrdinal("CompetitionName")),
                    CompetitionSlug = reader.GetString(reader.GetOrdinal("CompetitionSlug")),
                    Season = reader.IsDBNull(reader.GetOrdinal("SeasonId")) ? null : new Season
                    {
                        SeasonId = reader.GetInt32(reader.GetOrdinal("SeasonId")),
                        Name = reader.GetString(reader.GetOrdinal("SeasonName")),
                        StartDate = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("SeasonStartDate"))),
                        EndDate = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("SeasonEndDate")))
                    },
                },
                Sport = new Sport
                {
                    SportId = reader.GetInt32(reader.GetOrdinal("SportId")),
                    Name = reader.GetString(reader.GetOrdinal("SportName"))
                },
                Venue = new Venue
                {
                    VenueId = reader.GetInt32(reader.GetOrdinal("VenueId")),
                    Name = reader.GetString(reader.GetOrdinal("VenueName")),
                    Capacity = reader.GetInt32(reader.GetOrdinal("VenueCapacity")),
                    Address = new Address
                    {
                        AddressId = reader.GetInt32(reader.GetOrdinal("VenueAddressId")),
                        StreetNumber = reader.GetString(reader.GetOrdinal("VenueStreetNumber")),
                        StreetName = reader.GetString(reader.GetOrdinal("VenueStreet")),
                        City = reader.GetString(reader.GetOrdinal("VenueCity")),
                        Country = new Country
                        {
                            CountryId = reader.GetInt32(reader.GetOrdinal("VenueCountryId")),
                            Name = reader.GetString(reader.GetOrdinal("VenueCountryName")),
                            CountryCode = reader.GetString(reader.GetOrdinal("VenueCountryCode"))
                        }
                    }
                },
                Stage = new Stage
                {
                    StageId = reader.GetInt32(reader.GetOrdinal("StageId")),
                    Name = reader.GetString(reader.GetOrdinal("StageName")),
                    Ordering = reader.GetInt32(reader.GetOrdinal("StageOrdering"))
                }
            };
        }      
    }
}
