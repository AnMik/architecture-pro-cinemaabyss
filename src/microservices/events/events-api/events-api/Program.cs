using System.Text.Json;
using events_api;
using Microsoft.AspNetCore.Mvc;
using JsonOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<KafkaSender>();
builder.Services.Configure<JsonOptions>(x => x.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower);

var app = builder.Build();

app.MapGet("/api/events/health", () => Results.Ok(new Response.StatusResponse { Status = true }));

app.MapPost(
    "/api/events/movie",
    async ([FromServices] KafkaSender kafkaSender, [FromBody] Request.MovieCreated @event, CancellationToken ct) =>
    {
        try
        {
            var response = await kafkaSender.SendMovieCreated(@event, ct);
            return Results.Created(uri: default(string?), response);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Results.Json(new Response.ErrorResponse { Error = e.Message }, statusCode: 500);
        }
    });

app.MapPost("/api/events/user",
    async ([FromServices] KafkaSender kafkaSender, [FromBody] Request.UserCreated @event, CancellationToken ct) =>
    {
        try
        {
            var response = await kafkaSender.SendUserCreated(@event, ct);
            return Results.Created(uri: default(string?), response);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Results.Json(new Response.ErrorResponse { Error = e.Message }, statusCode: 500);
        }
    });

app.MapPost(
    "/api/events/payment",
    async ([FromServices] KafkaSender kafkaSender, [FromBody] Request.PaymentCreated @event, CancellationToken ct) =>
    {
        try
        {
            var response = await kafkaSender.SendPaymentCreated(@event, ct);
            return Results.Created(uri: default(string?), response);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Results.Json(new Response.ErrorResponse { Error = e.Message }, statusCode: 500);
        }
    });

app.Run("http://+:8082");

public static class KafkaSettings
{
    public static class Topics
    {
        public const string Movie = "movie-events";
        public const string User = "user-events";
        public const string Payment = "payment-events";
    }

    public static string GetServer() 
        => Environment.GetEnvironmentVariable("KAFKA_BROKERS")
           ?? throw new InvalidOperationException("Не задано значение KAFKA_BROKERS");
}