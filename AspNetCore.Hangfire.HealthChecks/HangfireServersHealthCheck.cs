using System;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using AspNetCore.Hangfire.HealthChecks.Models;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AspNetCore.Hangfire.HealthChecks;

public class HangfireServersHealthCheck : IHealthCheck
{
    private readonly HangfireOptions _hangfireOptions;

    public HangfireServersHealthCheck(HangfireOptions hangfireOptions)
    {
        _hangfireOptions = hangfireOptions;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_hangfireOptions.SqlConnectionString))
        {
            return HealthCheckResult.Unhealthy("The Hangfire server connection string is not configured.");
        }

        try
        {
            using var connection = new SqlConnection(_hangfireOptions.SqlConnectionString);
            await connection.OpenAsync(cancellationToken);
            var command = connection.CreateCommand();
            command.CommandText = $"SELECT [LastHeartbeat] FROM [{_hangfireOptions.SchemaName}].[Server]";
            var queryResult = await command.ExecuteReaderAsync(cancellationToken);
            if (queryResult.HasRows) // Corrected this line
            {
                // loop through the rows in the result set queryResult
                while (queryResult.Read())
                {
                    var lastHeartbeat = queryResult.GetSqlDateTime(0).Value;
                    if (lastHeartbeat < DateTime.UtcNow.AddMinutes(-_hangfireOptions.HeartbeatTimeout))
                    {
                        return HealthCheckResult.Unhealthy("One of the Hangfire servers is not healthy.");
                    }
                }
            }

            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("An exception was thrown while checking the Hangfire server health.", ex);
        }
    }
}
