using proxy_api;
using Yarp.ReverseProxy.Configuration;
using Yarp.ReverseProxy.LoadBalancing;

var builder = WebApplication.CreateBuilder(args);

var environmentVariables = new EnvironmentVariables();

builder.Services
    .AddSingleton(environmentVariables)
    .AddSingleton<ILoadBalancingPolicy, WeightedRandomLoadBalancingPolicy>()
    .AddReverseProxy()
    .LoadFromMemory(
        [
            new RouteConfig
            {
                RouteId = ProxyConfig.Routes.Users,
                ClusterId = ProxyConfig.Clusters.Monolith,
                Match = new RouteMatch { Path = "/api/users/{**any}", Methods = ["GET", "POST"] }
            },
            new RouteConfig
            {
                RouteId = ProxyConfig.Routes.Payments,
                ClusterId = ProxyConfig.Clusters.Monolith,
                Match = new RouteMatch { Path = "/api/payments/{**any}", Methods = ["GET", "POST"] }
            },
            new RouteConfig
            {
                RouteId = ProxyConfig.Routes.Subscriptions,
                ClusterId = ProxyConfig.Clusters.Monolith,
                Match = new RouteMatch { Path = "/api/subscription/{**any}", Methods = ["GET", "POST"] }
            },
            new RouteConfig
            {
                RouteId = ProxyConfig.Routes.Movies,
                ClusterId = ProxyConfig.Clusters.Movies,
                Match = new RouteMatch { Path = "/api/movies/{**any}", Methods = ["GET", "POST"] }
            },
            new RouteConfig
            {
                RouteId = ProxyConfig.Routes.Events,
                ClusterId = ProxyConfig.Clusters.Events,
                Match = new RouteMatch { Path = "/api/events/{**any}", Methods = ["GET", "POST"] }
            }
        ],
        [
            new ClusterConfig
            {
                ClusterId = ProxyConfig.Clusters.Monolith,
                Destinations = new Dictionary<string, DestinationConfig>
                {
                    {
                        ProxyConfig.Destinations.Monolith,
                        new DestinationConfig { Address = environmentVariables.MonolithUrl }
                    }
                }
            },
            new ClusterConfig
            {
                LoadBalancingPolicy = WeightedRandomLoadBalancingPolicy.PolicyName,
                ClusterId = ProxyConfig.Clusters.Movies,
                Destinations = new Dictionary<string, DestinationConfig>
                {
                    {
                        ProxyConfig.Destinations.Monolith,
                        new DestinationConfig { Address = environmentVariables.MonolithUrl }
                    },
                    {
                        ProxyConfig.Destinations.MovieService,
                        new DestinationConfig { Address = environmentVariables.MoviesServiceUrl }
                    }
                }
            },
            new ClusterConfig
            {
                ClusterId = ProxyConfig.Clusters.Events,
                Destinations = new Dictionary<string, DestinationConfig>
                {
                    {
                        ProxyConfig.Destinations.EventsService,
                        new DestinationConfig { Address = environmentVariables.EventsServiceUrl }
                    }
                }
            }
        ]);

var app = builder.Build();

app.MapGet("/health", () => Results.Ok("Strangler Fig Proxy is healthy"));

app.MapReverseProxy();

await app.RunAsync("http://+:8000");