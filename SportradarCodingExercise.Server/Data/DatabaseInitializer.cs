using Microsoft.Data.SqlClient;
using System.Text.Json;

namespace SportradarCodingExercise.Server.Data
{
    /// <summary>
    /// Handles database related initializations (databse, tables) and seeding sample data.
    /// </summary>
    public class DatabaseInitializer
    {
        private readonly string _connectionString;
        private readonly string _masterConnectionString;
        private readonly string _databaseName;
        private readonly List<string> requiredTables = new List<string>
        {
            "Season", "Competition", "Country", "Sport", "Team", "Status", 
            "Stage", "Address", "Venue", "EventType", "Event", "EventDetail"
        };

        /// <summary>
        /// Initializes a new instance of the DatabaseInitializer class.
        /// </summary>
        /// <param name="configuration">The configuration containing the connection string.</param>
        /// <exception cref="ArgumentNullException">Thrown when configuration or DefaultConnection string is null.</exception>
        public DatabaseInitializer(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new ArgumentNullException(nameof(configuration));

            var builder = new SqlConnectionStringBuilder(_connectionString);

            _databaseName = builder.InitialCatalog;

            builder.InitialCatalog = "master";

            _masterConnectionString = builder.ConnectionString;

        }

        /// <summary>
        /// Initializes the database structure by creating the database and required tables.
        /// </summary>
        /// <returns>A task that represents the asynchronous database initialization.</returns>
        public async Task InitializeAsync()
        {
            await EnsureDatabaseCreatedAsync();

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            if (await TablesExistAsync(connection))
            {
                return;
            }

            using var transaction = connection.BeginTransaction();
            try
            {
                await CreateTablesAsync(connection, transaction);
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
            }
        }

        /// <summary>
        /// Seeds the database with initial data from a JSON file.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <exception cref="FileNotFoundException">Thrown when seed-data.json is not found.</exception>
        /// <exception cref="JsonException">Thrown when the JSON file cannot be parsed.</exception>
        public async Task SeedAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var jsonPath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "seed-data.json");

            if (!File.Exists(jsonPath))
            {
                throw new FileNotFoundException("Seed data file not found", jsonPath);
            }

            var jsonContent = await File.ReadAllTextAsync(jsonPath);
            JsonDocument? jsonDocument = JsonSerializer.Deserialize<JsonDocument>(jsonContent);

            if (jsonDocument == null)
            {
                throw new JsonException("Failed to parse seed data file");
            }


            using var transaction = connection.BeginTransaction();
            try
            {
                foreach (var tableName in requiredTables)
                {
                    await SeedTableFromJsonAsync(connection, transaction, tableName, jsonDocument.RootElement.GetProperty(tableName));
                }

                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
            }
        }

        /// <summary>
        /// Ensures that the database exists, creates it if it doesn't.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <remarks>
        /// Connects to the master database to check for the existance of the desired database.
        /// </remarks>
        private async Task EnsureDatabaseCreatedAsync()
        {
            using var connection = new SqlConnection(_masterConnectionString);
            await connection.OpenAsync();

            var checkDbCommand = new SqlCommand(
                "SELECT database_id FROM sys.databases WHERE Name = @dbName",
                connection);

            checkDbCommand.Parameters.AddWithValue("@dbName", _databaseName);

            if (await checkDbCommand.ExecuteScalarAsync() == null)
            {
                var createDbCommand = new SqlCommand(
                    $"CREATE DATABASE {_databaseName}", connection);

                await createDbCommand.ExecuteNonQueryAsync();
            }
        }

        /// <summary>
        /// Checks if all required tables exist in the database.
        /// </summary>
        /// <param name="connection">The SQL connection to use.</param>
        /// <returns>
        /// True if all required tables exist; otherwise, false.
        /// </returns>
        private async Task<bool> TablesExistAsync(SqlConnection connection)
        {
            const string sql = @"
                SELECT COUNT(DISTINCT TABLE_NAME)
                FROM INFORMATION_SCHEMA.TABLES 
                WHERE TABLE_TYPE = 'BASE TABLE' 
                AND TABLE_NAME IN ({0})";

            var parameters = requiredTables
                .Select((name, index) => $"@p{index}")
                .ToList();

            using var command = new SqlCommand(
                string.Format(sql, string.Join(",", parameters)),
                connection);

            for (int i = 0; i < requiredTables.Count; i++)
            {
                command.Parameters.AddWithValue($"@p{i}", requiredTables[i]);
            }

            var tablesFound = Convert.ToInt32(await command.ExecuteScalarAsync());
            return tablesFound == requiredTables.Count;
        }

        /// <summary>
        /// Seeds a table with data from a JSON element.
        /// </summary>
        /// <param name="connection">The SQL connection to use.</param>
        /// <param name="transaction">The transaction to use for the seeding operation.</param>
        /// <param name="tableName">The name of the table to seed.</param>
        /// <param name="data">The JSON data to seed the table with.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <remarks>
        /// This method temporarily enables IDENTITY_INSERT to allow explicit ID values,
        /// inserts the data, then disables IDENTITY_INSERT.
        /// </remarks>
        private static async Task SeedTableFromJsonAsync(SqlConnection connection, SqlTransaction transaction, string tableName, JsonElement data)
        {
            if (!data.EnumerateArray().Any()) return;

            var firstItem = data.EnumerateArray().First();
            var columns = string.Join(", ", firstItem.EnumerateObject().Select(p => p.Name));

            using (var identityCommand = new SqlCommand($"SET IDENTITY_INSERT {tableName} ON", connection, transaction))
            {
                await identityCommand.ExecuteNonQueryAsync();
            }

            try
            {
                var valuesList = new List<string>();
                var sqlParameters = new List<SqlParameter>();
                var parameterIndex = 0;

                foreach (var item in data.EnumerateArray())
                {
                    var parameters = new List<string>();
                    foreach (var property in item.EnumerateObject())
                    {
                        var paramName = $"@p{parameterIndex}";
                        parameters.Add(paramName);
                        var value = property.Value.ValueKind == JsonValueKind.Null ?
                            DBNull.Value :
                            ConvertJsonValue(property.Value);
                        sqlParameters.Add(new SqlParameter(paramName, value));
                        parameterIndex++;
                    }
                    valuesList.Add($"({string.Join(", ", parameters)})");
                }

                var sql = $@"
                    INSERT INTO {tableName} ({columns})
                    VALUES {string.Join(",\n", valuesList)}";

                using var command = new SqlCommand(sql, connection, transaction);
                command.Parameters.AddRange(sqlParameters.ToArray());
                await command.ExecuteNonQueryAsync();
            }
            finally
            {
                using var identityCommand = new SqlCommand($"SET IDENTITY_INSERT {tableName} OFF", connection, transaction);
                await identityCommand.ExecuteNonQueryAsync();
            }
        }

        /// <summary>
        /// Converts a JsonElement to its appropriate type.
        /// </summary>
        /// <param name="element">The JsonElement to convert.</param>
        /// <returns>
        /// The converted value as an object, or DBNull.Value for null elements.
        /// </returns>
        private static object ConvertJsonValue(JsonElement element)
        {
            if (element.ValueKind == JsonValueKind.Null)
                return DBNull.Value;

            return element.ValueKind switch
            {
                JsonValueKind.String => element.GetString() is { } s ? s : DBNull.Value,
                JsonValueKind.Number => element.GetDecimal(),
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                _ => DBNull.Value
            };
        }

        /// <summary>
        /// Creates all required database tables with their relationships.
        /// </summary>
        /// <param name="connection">The SQL connection to use.</param>
        /// <param name="transaction">The transaction to use for creating tables.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        /// <remarks>
        /// Creates tables in the correct order to satisfy foreign key constraints.
        /// Tables are created with IDENTITY columns and appropriate relationships.
        /// </remarks>
        private static async Task CreateTablesAsync(SqlConnection connection, SqlTransaction transaction)
        {
            using var command = connection.CreateCommand();
            command.Transaction = transaction;

            command.CommandText = @"
            CREATE TABLE Season (
                SeasonId INT PRIMARY KEY IDENTITY(1,1),
                StartDate DATE NOT NULL,
                EndDate DATE NOT NULL,
                Name VARCHAR(100) NOT NULL
            );

            CREATE TABLE Competition (
                CompetitionId INT PRIMARY KEY IDENTITY(1,1),
                Name VARCHAR(100) NOT NULL,
                CompetitionSlug VARCHAR(100) UNIQUE NOT NULL,
                SeasonId INT NULL,
                CONSTRAINT _fk_competition_season FOREIGN KEY (SeasonId) 
                    REFERENCES Season(SeasonId)
            );

            CREATE TABLE Stage (
                StageId INT PRIMARY KEY IDENTITY(1,1),
                Name VARCHAR(100) NOT NULL,
                Ordering INT NOT NULL
            );

            CREATE TABLE Status (
                StatusId INT PRIMARY KEY IDENTITY(1,1),
                Name VARCHAR(50) NOT NULL
            );

            CREATE TABLE Country (
                CountryId INT PRIMARY KEY IDENTITY(1,1),
                Name VARCHAR(50) NOT NULL,
                CountryCode CHAR(3) UNIQUE NOT NULL
            );

            CREATE TABLE Sport (
                SportId INT PRIMARY KEY IDENTITY(1,1),
                Name VARCHAR(50) NOT NULL
            );

            CREATE TABLE Team (
                TeamId INT PRIMARY KEY IDENTITY(1,1),
                Name VARCHAR(50) NOT NULL,
                OfficialName VARCHAR(100) NOT NULL,
                Slug VARCHAR(100) UNIQUE NOT NULL,
                Abbreviation CHAR(3) NOT NULL,
                CountryId INT NOT NULL,
                SportId INT NOT NULL,
                CONSTRAINT _fk_team_country FOREIGN KEY (CountryId) 
                    REFERENCES Country(CountryId),
                CONSTRAINT _fk_team_sport FOREIGN KEY (SportId)
                    REFERENCES Sport(SportId),
                CONSTRAINT _uk_team_sport_abbreviation UNIQUE (SportId, Abbreviation)
            );

            CREATE TABLE Address (
                AddressId INT PRIMARY KEY IDENTITY(1,1),
                StreetNumber VARCHAR(20) NOT NULL,
                StreetName VARCHAR(100) NOT NULL,
                City VARCHAR(50) NOT NULL,
                CountryId INT NOT NULL,
                CONSTRAINT _fk_address_country FOREIGN KEY (CountryId) 
                    REFERENCES Country(CountryId)
            );

            CREATE TABLE Venue (
                VenueId INT PRIMARY KEY IDENTITY(1,1),
                Name VARCHAR(100) NOT NULL,
                AddressId INT NOT NULL,
                Capacity INT NOT NULL,
                CONSTRAINT _fk_venue_address FOREIGN KEY (AddressId) 
                    REFERENCES Address(AddressId)
            );

            CREATE TABLE EventType (
                EventTypeId INT PRIMARY KEY IDENTITY(1,1),
                Name VARCHAR(50) UNIQUE NOT NULL
            );

            CREATE TABLE Event (
                EventId INT PRIMARY KEY IDENTITY(1,1),
                Date DATE NOT NULL,
                TimeUTC TIME NOT NULL,
                DurationInMinutes INT NOT NULL,
                Description VARCHAR(1000) NULL,
                HomeScore INT NOT NULL,
                AwayScore INT NOT NULL,
                StatusId INT NOT NULL,
                HomeTeamId INT NOT NULL,
                AwayTeamId INT NOT NULL,
                VenueId INT NOT NULL,
                StageId INT NOT NULL,
                CompetitionId INT NOT NULL,
                SportId INT NOT NULL,  
                CONSTRAINT _fk_event_status FOREIGN KEY (StatusId) 
                    REFERENCES Status(StatusId),
                CONSTRAINT _fk_event_hometeam FOREIGN KEY (HomeTeamId) 
                    REFERENCES Team(TeamId),
                CONSTRAINT _fk_event_awayteam FOREIGN KEY (AwayTeamId) 
                    REFERENCES Team(TeamId),
                CONSTRAINT _fk_event_venue FOREIGN KEY (VenueId) 
                    REFERENCES Venue(VenueId),
                CONSTRAINT _fk_event_stage FOREIGN KEY (StageId) 
                    REFERENCES Stage(StageId),
                CONSTRAINT _fk_event_competition FOREIGN KEY (CompetitionId) 
                    REFERENCES Competition(CompetitionId),
                CONSTRAINT _fk_event_sport FOREIGN KEY (SportId)
                    REFERENCES Sport(SportId)
            );

            CREATE TABLE EventDetail (
                EventDetailId INT PRIMARY KEY IDENTITY(1,1),
                EventId INT NOT NULL,
                TeamId INT NOT NULL,
                EventTypeId INT NOT NULL,
                RecordedAtUTC DATETIME NOT NULL,
                Description VARCHAR(500) NULL,
                CONSTRAINT _fk_eventdetail_event FOREIGN KEY (EventId) 
                    REFERENCES Event(EventId),
                CONSTRAINT _fk_eventdetail_team FOREIGN KEY (TeamId) 
                    REFERENCES Team(TeamId),
                CONSTRAINT _fk_eventdetail_eventtype FOREIGN KEY (EventTypeId) 
                    REFERENCES EventType(EventTypeId)
            );

            CREATE INDEX IDX_Event_SportId_Date ON Event(SportId, Date);
            CREATE INDEX IDX_Team_SportId ON Team(SportId);

             ";


            await command.ExecuteNonQueryAsync();
        }
    }
}