namespace SportradarCodingExercise.Server.Models
{
    public class Event
    {
        public required int EventId { get; set; }
        public required DateOnly Date { get; set; }
        public required TimeOnly TimeUTC { get; set; }
        public required int DurationInMinutes { get; set; }
        public string? Description { get; set; }
        public required int HomeScore { get; set; }
        public required int AwayScore { get; set; }
        public int? WinnerTeamId { get; set; }

        public required Status Status { get; set; }
        public required Team HomeTeam { get; set; }
        public required Team AwayTeam { get; set; }
        public required Venue Venue { get; set; }
        public required Stage Stage { get; set; }
        public required Competition Competition { get; set; }
        public required Sport Sport { get; set; }
    }
}
