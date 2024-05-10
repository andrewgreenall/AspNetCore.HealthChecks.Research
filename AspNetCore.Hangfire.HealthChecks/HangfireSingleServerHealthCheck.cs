using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AspNetCore.Hangfire.HealthChecks.Models;
using AspNetCore.Hangfire.HealthChecks.Models.Data;
using AspNetCore.Hangfire.HealthChecks.Services;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AspNetCore.Hangfire.HealthChecks;

public class HangfireSingleServerHealthCheck : IHealthCheck
{
    private readonly HangfireOptions _hangfireOptions;
    private readonly IHangfireDataService _hangfireDataService;

    public HangfireSingleServerHealthCheck(HangfireOptions hangfireOptions, IHangfireDataService hangfireDataService)
    {
        _hangfireOptions = hangfireOptions;
        _hangfireDataService = hangfireDataService;
    }
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_hangfireOptions.SqlConnectionString))
        {
            return HealthCheckResult.Unhealthy("The Hangfire server connection string is not configured.");
        }

        try
        {
            var servers = (await _hangfireDataService.GetServers(cancellationToken)).ToDtoModel();
            var checkHealthy = servers.Any(s => s.ServiceName == _hangfireOptions.ServerName 
                && s.LastHeartbeat > DateTime.UtcNow.AddMinutes(-_hangfireOptions.HeartbeatTimeout)
                && (string.IsNullOrEmpty(_hangfireOptions.MachineName) || s.MachineName == _hangfireOptions.MachineName));
            if (checkHealthy)
            {
                return HealthCheckResult.Healthy();
            }
            var checkUnhealthyResults = servers.Where(s => s.ServiceName == _hangfireOptions.ServerName 
                && s.LastHeartbeat < DateTime.UtcNow.AddMinutes(-_hangfireOptions.HeartbeatTimeout)
                && (string.IsNullOrEmpty(_hangfireOptions.MachineName) || s.MachineName == _hangfireOptions.MachineName));
            var checkUnhealthy = checkUnhealthyResults.Any();
            if (checkUnhealthy)
            {
                var lastHeartbeat = checkUnhealthyResults.Max(uhr => uhr.LastHeartbeat);
                return HealthCheckResult.Unhealthy($"Last heartbeat was {lastHeartbeat.ToLongDateString()} {lastHeartbeat.ToShortTimeString()}.");
            }

            return HealthCheckResult.Unhealthy("No results found within the timeout.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("An exception was thrown while checking the Hangfire server health.", ex);
        }
    }
}
