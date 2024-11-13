namespace SportradarCodingExercise.Server.Models
{
    public class Country
    {
        public int CountryId { get; set; }
        public required string Name { get; set; }
        public required string CountryCode { get; set; }
    }
}
