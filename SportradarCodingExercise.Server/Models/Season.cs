namespace SportradarCodingExercise.Server.Models
{
    public class Season
    {
        public int SeasonId { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public required string Name { get; set; }
    }
}
