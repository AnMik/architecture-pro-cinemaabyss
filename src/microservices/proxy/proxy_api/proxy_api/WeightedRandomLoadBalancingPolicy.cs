using Yarp.ReverseProxy.LoadBalancing;
using Yarp.ReverseProxy.Model;

namespace proxy_api;

public sealed class WeightedRandomLoadBalancingPolicy : ILoadBalancingPolicy
{
    public const string PolicyName = "WeightedRandom";

    public string Name => PolicyName;

    private readonly double _migrationPercent;

    public WeightedRandomLoadBalancingPolicy(EnvironmentVariables environmentVariables)
        => _migrationPercent = environmentVariables is { GradualMigration: true, MoviesMigrationPercent: { } percent }
            ? percent
            : 100;

    public DestinationState? PickDestination(
        HttpContext context,
        ClusterState cluster,
        IReadOnlyList<DestinationState> availableDestinations)
    {
        var val = Random.Shared.NextDouble() * 100;

        var destinationId = val < _migrationPercent
            ? ProxyConfig.Destinations.MovieService
            : ProxyConfig.Destinations.Monolith;

        return FindDestination(availableDestinations, destinationId) ?? availableDestinations[0];
    }

    private static DestinationState? FindDestination(IReadOnlyList<DestinationState> destinations, string destinationId)
        => destinations.FirstOrDefault(x => x.DestinationId == destinationId);
}