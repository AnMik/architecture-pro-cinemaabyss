namespace events_api;

public abstract class Request
{
    public sealed class MovieCreated
    {
        public int MovieId { get; init; }
        public string? Title { get; init; }
        public string? Action { get; init; }
        public int UserId { get; init; }
        public decimal Rating { get; init; }
        public string[]? Genres { get; init; }
        public string? Description { get; init; }
    }

    public sealed class UserCreated
    {
        public int UserId { get; init; }
        public string? Username { get; init; }
        public string? Email { get; init; }
        public string? Action { get; init; }
        public DateTime Timestamp { get; init; }
    }

    public sealed class PaymentCreated
    {
        public int PaymentId { get; init; }
        public int UserId { get; init; }
        public decimal Amount { get; init; }
        public string? Status { get; init; }
        public DateTime Timestamp { get; init; }
        public string? MethodType { get; init; }
    }
}