namespace proxy_api;

public sealed class EnvironmentVariables
{
    public string MonolithUrl => Environment.GetEnvironmentVariable("MONOLITH_URL")
                                 ?? throw new InvalidOperationException("Не указан MONOLITH_URL");

    public string MoviesServiceUrl => Environment.GetEnvironmentVariable("MOVIES_SERVICE_URL")
                                      ?? throw new InvalidOperationException("Не указан MOVIES_SERVICE_URL");

    public string EventsServiceUrl => Environment.GetEnvironmentVariable("EVENTS_SERVICE_URL")
                                      ?? throw new InvalidOperationException("Не указан vEVENTS_SERVICE_URL");

    public bool? GradualMigration =>
        bool.TryParse(Environment.GetEnvironmentVariable("GRADUAL_MIGRATION"), out var gradualMigration)
            ? gradualMigration
            : null;

    public double? MoviesMigrationPercent =>
        double.TryParse(Environment.GetEnvironmentVariable("MOVIES_MIGRATION_PERCENT"), out var moviesMigrationPercent)
            ? moviesMigrationPercent
            : null;
}