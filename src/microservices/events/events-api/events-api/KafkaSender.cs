using System.Text.Json;
using Confluent.Kafka;

namespace events_api;

public sealed class KafkaSender : IDisposable
{
    private readonly Lazy<IProducer<Null, string>> _producer =
        new(() => new ProducerBuilder<Null, string>(CreateProducerConfig()).Build());

    private readonly Lazy<IConsumer<Null, string>> _consumer =
        new(() =>
        {
            var consumer = new ConsumerBuilder<Null, string>(CreateConsumerConfig()).Build();

            consumer.Subscribe([
                KafkaSettings.Topics.Movie,
                KafkaSettings.Topics.User,
                KafkaSettings.Topics.Payment
            ]);

            return consumer;
        });

    private static ProducerConfig CreateProducerConfig() => new() { BootstrapServers = KafkaSettings.GetServer() };

    private static ConsumerConfig CreateConsumerConfig()
        => new()
        {
            GroupId = "default-consumer-group",
            BootstrapServers = KafkaSettings.GetServer(),
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = true
        };

    public async Task<Response.EventResponse> SendMovieCreated(Request.MovieCreated @event, CancellationToken ct)
    {
        var type = "movie";
        await _producer.Value.ProduceAsync(KafkaSettings.Topics.Movie, CreateMessage(@event), ct);

        var result = _consumer.Value.Consume(ct);

        var deserializedEvent = JsonSerializer.Deserialize<Request.MovieCreated>(result.Message.Value);

        return new Response.EventResponse
        {
            Partition = result.Partition,
            Offset = result.Offset.Value,
            Status = "success",
            Event = new Response.EventResponse.EventData
            {
                Id = $"{type}-{deserializedEvent.MovieId}-{deserializedEvent.Action}",
                Type = type,
                Payload = new Response.EventResponse.EventData.EventPayload(),
                Timestamp = result.Message.Timestamp.UtcDateTime
            }
        };
    }

    public async Task<Response.EventResponse> SendUserCreated(Request.UserCreated @event, CancellationToken ct)
    {
        var type = "user";
        await _producer.Value.ProduceAsync(KafkaSettings.Topics.User, CreateMessage(@event), ct);

        var result = _consumer.Value.Consume(ct);

        var deserializedEvent = JsonSerializer.Deserialize<Request.UserCreated>(result.Message.Value);

        return new Response.EventResponse
        {
            Partition = result.Partition,
            Offset = result.Offset.Value,
            Status = "success",
            Event = new Response.EventResponse.EventData
            {
                Id = $"{type}-{deserializedEvent.UserId}-{deserializedEvent.Action}",
                Type = type,
                Payload = new Response.EventResponse.EventData.EventPayload(),
                Timestamp = result.Message.Timestamp.UtcDateTime
            }
        };
    }

    public async Task<Response.EventResponse> SendPaymentCreated(Request.PaymentCreated @event, CancellationToken ct)
    {
        var type = "payment";
        await _producer.Value.ProduceAsync(KafkaSettings.Topics.Payment, CreateMessage(@event), ct);

        var result = _consumer.Value.Consume(ct);

        var deserializedEvent = JsonSerializer.Deserialize<Request.PaymentCreated>(result.Message.Value);

        return new Response.EventResponse
        {
            Partition = result.Partition,
            Offset = result.Offset.Value,
            Status = "success",
            Event = new Response.EventResponse.EventData
            {
                Id = $"{type}-{deserializedEvent.PaymentId}-{deserializedEvent.Status}",
                Type = type,
                Payload = new Response.EventResponse.EventData.EventPayload(),
                Timestamp = result.Message.Timestamp.UtcDateTime
            }
        };
    }

    public void Dispose()
    {
        if (_producer.IsValueCreated)
        {
            _producer.Value.Dispose();
        }

        if (_consumer.IsValueCreated)
        {
            _consumer.Value.Dispose();
        }
    }

    private static Message<Null, string> CreateMessage<T>(T @event)
        => new() { Value = JsonSerializer.Serialize(@event) };
}