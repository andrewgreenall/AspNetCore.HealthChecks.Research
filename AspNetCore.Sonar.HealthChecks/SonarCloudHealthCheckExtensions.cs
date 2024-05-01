using AspNetCore.Sonar.HealthChecks.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AspNetCore.Sonar.HealthChecks;

public static class SonarCloudHealthCheckExtensions
{
    /// <summary>
    /// Adds a health check for Sonar Cloud project status. This will return unhealthy if any of the project status conditions are in an error or warning state.
    /// </summary>
    /// <param name="builder">The health check builder.</param>
    /// <param name="setup">An optional action to configure Hangfire options.</param>
    /// <param name="name">The name of the health check.</param>
    /// <param name="failureStatus">The health status to return when the health check fails.</param>
    /// <param name="tags">The tags associated with the health check.</param>
    /// <param name="timeout">The timeout for the health check.</param>
    /// <returns>The health check builder.</returns>
    public static IHealthChecksBuilder AddSonarCloudProjectHealthCheck(
            this IHealthChecksBuilder builder,
            Action<SonarCloudOptions> setup,
            string name,
            HealthStatus? failureStatus = default,
            IEnumerable<string> tags = default,
            TimeSpan? timeout = default)
    {
        var sonarCloudOptions = new SonarCloudOptions();
        setup?.Invoke(sonarCloudOptions);

        return builder.Add(new HealthCheckRegistration(
           name,
           sp => new SonarCloudProjectHealthCheck(sonarCloudOptions, sp.GetRequiredService<HttpClient>(), sp.GetRequiredService<IMemoryCache>()),
           failureStatus,
           tags,
           timeout));
    }
}
