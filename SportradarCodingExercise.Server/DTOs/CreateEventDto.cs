using SportradarCodingExercise.Server.Models;

namespace SportradarCodingExercise.Server.DTOs
{
    public class CreateEventDto
    {
        public required DateOnly Date { get; set; }
        public required TimeOnly TimeUTC { get; set; }
        public required int DurationInMinutes { get; set; }
        public string? Description { get; set; }
        public required int HomeScore { get; set; }
        public required int AwayScore { get; set; }

        public required int StatusId { get; set; }
        public required int HomeTeamId { get; set; }
        public required int AwayTeamId { get; set; }
        public required int VenueId { get; set; }
        public required int StageId { get; set; }
        public required int CompetitionId { get; set; }
        public required int SportId { get; set; }
    }
}
