namespace SportradarCodingExercise.Server.Models
{
    public class Stage
    {
        public int StageId { get; set; }
        public required string Name { get; set; }
        public int Ordering { get; set; }
    }
}
