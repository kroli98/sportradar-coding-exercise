namespace SportradarCodingExercise.Server.Models
{
    public class Venue
    {
        public required int VenueId { get; set; }
        public required string Name { get; set; }
        public required int Capacity { get; set; }

        public required Address Address { get; set; }
    }
}
