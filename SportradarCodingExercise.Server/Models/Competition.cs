﻿namespace SportradarCodingExercise.Server.Models
{
    public class Competition
    {
        public int CompetitionId { get; set; }
        public required string Name { get; set; }
        public required string CompetitionSlug { get; set; }

        public Season? Season { get; set; }
    }
}