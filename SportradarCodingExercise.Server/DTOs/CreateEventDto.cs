using System.ComponentModel.DataAnnotations;

namespace SportradarCodingExercise.Server.DTOs
{
    public class CreateEventDto
    {
        public required DateOnly Date { get; set; }
        public required TimeOnly TimeUTC { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Only numbers greater than 1 are allowed")]
        public required int DurationInMinutes { get; set; }

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters")]
        public string? Description { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Only numbers greater than or equal to 0 are allowed")]
        public required int HomeScore { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Only numbers greater than or equal to 0 are allowed")]
        public required int AwayScore { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Only numbers greater than 1 are allowed")]
        public required int StatusId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Only numbers greater than 1 are allowed")]
        public required int HomeTeamId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Only numbers greater than 1 are allowed")]
        public required int AwayTeamId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Only numbers greater than 1 are allowed")]
        public required int VenueId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Only numbers greater than 1 are allowed")]
        public required int StageId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Only numbers greater than 1 are allowed")]
        public required int CompetitionId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Only numbers greater than 1 are allowed")]
        public required int SportId { get; set; }
    }
}
