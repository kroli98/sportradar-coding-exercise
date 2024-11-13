namespace SportradarCodingExercise.Server.Models
{
    public class EventDetail
    {
        public int EventDetailId { get; set; }
        public DateTime RecordedAtUTC { get; set; }
        public string? Description { get; set; }

        public required Event Event { get; set; }
        public required Team Team { get; set; }
        public required EventType EventType { get; set; }
    }
}
