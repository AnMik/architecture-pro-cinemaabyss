namespace proxy_api;

public static class ProxyConfig
{
    public static class Destinations
    {
        public const string Monolith = "destination1";
        public const string MovieService = "destination2";
        public const string EventsService = "destination3";
    }

    public static class Routes
    {
        public const string Users = "usersRoute";
        public const string Payments = "paymentsRoute";
        public const string Subscriptions = "subscriptionsRoute";
        public const string Movies = "moviesRoute";
        public const string Events = "eventsRoute";
    }

    public static class Clusters
    {
        public const string Movies = "moviesCluster";
        public const string Events = "eventsCluster";
        public const string Monolith = "monolithCluster";
    }
}