using SportradarCodingExercise.Server.Models;

namespace SportradarCodingExercise.Server.DTOs
{
    public class EventDetailDto
    {
        public required int EventDetailId { get; set; }
        public required DateTime RecordedAtUTC { get; set; }
        public string? Description { get; set; }
        public required Team Team { get; set; }
        public required EventType EventType { get; set; }
    }
}
