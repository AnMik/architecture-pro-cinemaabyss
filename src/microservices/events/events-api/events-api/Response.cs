namespace events_api;

public abstract class Response
{
    public sealed class StatusResponse
    {
        public bool Status { get; init; }

    }
    public sealed class ErrorResponse
    {
        public string Error { get; init; }
    }
    
    public sealed class EventResponse
    {
        public sealed class EventData
        {
            public sealed class EventPayload
            {
                
            }

            public string Id { get; init; }
            public string Type { get; init; }
            public DateTime Timestamp { get; init; }
            public EventPayload Payload { get; init; }
        }
        
        public string Status { get; init; }
        public int Partition { get; init; }
        public long Offset { get; init; }
        public EventData Event { get; init; }
    }
}