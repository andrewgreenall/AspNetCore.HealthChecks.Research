using System;
using System.Collections.Generic;
using AspNetCore.Hangfire.HealthChecks.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AspNetCore.Hangfire.HealthChecks;

public static class HangfireHealthCheckExtensions
{
    /// <summary>
    /// Adds a health check for Hangfire servers to the health check builder.
    /// </summary>
    /// <param name="builder">The health check builder.</param>
    /// <param name="setup">An optional action to configure Hangfire options.</param>
    /// <param name="name">The name of the health check.</param>
    /// <param name="failureStatus">The health status to return when the health check fails.</param>
    /// <param name="tags">The tags associated with the health check.</param>
    /// <param name="timeout">The timeout for the health check.</param>
    /// <returns>The health check builder.</returns>
    public static IHealthChecksBuilder AddHangfireServers(
            this IHealthChecksBuilder builder,
            Action<HangfireOptions> setup,
            string name,
            HealthStatus? failureStatus = default,
            IEnumerable<string> tags = default,
            TimeSpan? timeout = default)
    {
        var hangfireOptions = new HangfireOptions();
        setup?.Invoke(hangfireOptions);

        return builder.Add(new HealthCheckRegistration(
           name,
           sp => new HangfireServersHealthCheck(hangfireOptions),
           failureStatus,
           tags,
           timeout));
    }
}
