namespace SportradarCodingExercise.Server.Models
{
    public class Address
    {
        public required int AddressId { get; set; }
        public required string StreetNumber { get; set; }
        public required string StreetName { get; set; }
        public required string City { get; set; }

        public required Country Country { get; set; }
    }
}
