using System;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using AspNetCore.Hangfire.HealthChecks.Models;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace AspNetCore.Hangfire.HealthChecks;

public class HangfireSingleServerHealthCheck : IHealthCheck
{
    private readonly HangfireOptions _hangfireOptions;

    public HangfireSingleServerHealthCheck(HangfireOptions hangfireOptions)
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
            command.CommandText = $"SELECT [Id], [Data], [LastHeartbeat] FROM [{_hangfireOptions.SchemaName}].[Server] with (nolock)";
            var queryResult = await command.ExecuteReaderAsync(cancellationToken);
            if (queryResult.HasRows) // Corrected this line
            {
                // loop through the rows in the result set queryResult
                while (queryResult.Read())
                {
                    var ServerProcessor = new HangfireFactory().CreateHangfireProcessor("ServerEntity");
                    var ServerIdProcessor = new HangfireFactory().CreateHangfireProcessor("ServerId");
                    var serverEntity = (ServerEntityResult)ServerProcessor.ProcessData(queryResult);

                    var serverIdProcessorResult = (ServerIdModelResult)ServerIdProcessor.ProcessData(serverEntity.Result.Id);
                    var serverId = serverIdProcessorResult.Result.ServiceName;
                    var machineName = serverIdProcessorResult.Result.MachineName;
                    var lastHeartbeat = serverEntity.Result.LastHeartbeat;

                    if (serverId == _hangfireOptions.ServerName && lastHeartbeat > DateTime.UtcNow.AddMinutes(-_hangfireOptions.HeartbeatTimeout))
                    {
                        // if the machine name is empty (Optional parameter, first instance used) 
                        // or the machine name is the same as the current machine name
                        if(string.IsNullOrEmpty(machineName) || machineName == _hangfireOptions.MachineName)
                        {
                            return HealthCheckResult.Healthy();
                        }
                    }
                    else if (serverId == _hangfireOptions.ServerName 
                        && lastHeartbeat < DateTime.UtcNow.AddMinutes(-_hangfireOptions.HeartbeatTimeout)
                        && machineName == _hangfireOptions.MachineName)
                    {
                        return HealthCheckResult.Unhealthy($"Last heartbeat was {lastHeartbeat.ToLongDateString()} {lastHeartbeat.ToShortTimeString()}.");
                    }
                }
            }

            return HealthCheckResult.Unhealthy("No results found within the timeout.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("An exception was thrown while checking the Hangfire server health.", ex);
        }
    }
}
