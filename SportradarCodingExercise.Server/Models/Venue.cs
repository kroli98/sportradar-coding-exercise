namespace SportradarCodingExercise.Server.Models
{
    public class Venue
    {
        public int VenueId { get; set; }
        public required string Name { get; set; }
        public int Capacity { get; set; }

        public required Address Address { get; set; }
    }
}
