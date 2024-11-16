namespace SportradarCodingExercise.Server.Models
{
    public class Season
    {
        public required int SeasonId { get; set; }
        public required DateOnly StartDate { get; set; }
        public required DateOnly EndDate { get; set; }
        public required string Name { get; set; }
    }
}
