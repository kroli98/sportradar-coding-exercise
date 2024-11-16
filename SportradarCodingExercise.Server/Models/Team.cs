namespace SportradarCodingExercise.Server.Models
{
    public class Team
    {
        public required int TeamId { get; set; }
        public required string Name { get; set; }
        public required string OfficialName { get; set; }
        public required string Slug { get; set; }
        public required string Abbreviation { get; set; }

        public required Country Country { get; set; }
        public required Sport Sport { get; set; }
    }
}
