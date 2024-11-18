namespace SportradarCodingExercise.Server.CustomExceptions
{
    public class EventConflictException : Exception
    {
        public EventConflictType ConflictType { get; }

        public EventConflictException(string message, EventConflictType conflictType)
            : base(message)
        {
            ConflictType = conflictType;
        }
    }

    public enum EventConflictType
    {
        Venue,
        Team
    }
}
