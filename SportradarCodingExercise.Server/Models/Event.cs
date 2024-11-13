namespace SportradarCodingExercise.Server.Models
{
    public class Event
    {
        public int EventId { get; set; }
        public DateOnly Date { get; set; }
        public TimeOnly TimeUTC { get; set; }
        public int DurationInMinutes { get; set; }
        public string? Description { get; set; }
        public int HomeScore { get; set; }
        public int AwayScore { get; set; }

        public required Status Status { get; set; }
        public required Team HomeTeam { get; set; }
        public required Team AwayTeam { get; set; }
        public required Venue Venue { get; set; }
        public required Stage Stage { get; set; }
        public required Competition Competition { get; set; }
        public required Sport Sport { get; set; }
    }
}
