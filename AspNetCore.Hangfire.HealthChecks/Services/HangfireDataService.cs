using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using AspNetCore.Hangfire.HealthChecks.Models.Data;

namespace AspNetCore.Hangfire.HealthChecks.Services;

public class HangfireDataService : IHangfireDataService, IDisposable
{
    private readonly string _connectionString;
    private readonly string _schemaName;
    private readonly SqlConnection _connection;

    public HangfireDataService(string connectionString, string schemaName)
    {
        _connectionString = connectionString;
        _schemaName = schemaName;
        _connection = new SqlConnection(connectionString);
    }

    public void Dispose()
    {
        _connection.Close();
    }

    public async Task OpenConnectionAsync()
    {
        if (_connection.State == System.Data.ConnectionState.Closed)
        {
            await _connection.OpenAsync();
        }
    }

    public async Task<IList<Server>> GetServers(CancellationToken cancellationToken = default)
    {
        await OpenConnectionAsync();
        var result = new List<Server>();

        var command = _connection.CreateCommand();
        command.CommandText = $"SELECT [Id], [Data], [LastHeartbeat] FROM [{_schemaName}].[Server] with (nolock)";
        var queryResult = await command.ExecuteReaderAsync(cancellationToken);
        if (queryResult.HasRows)
        {
            // loop through the rows in the result set queryResult
            while (queryResult.Read())
            {
                var ServerProcessor = new HangfireFactory().CreateHangfireProcessor("ServerEntity");
                var serverEntity = (ServerEntityResult)ServerProcessor.ProcessData(queryResult);
                result.Add(serverEntity.Result);
            }
        }

        return result;
    }

    /// <summary>
    /// List Data sources
    /// </summary>
    public Task<IList<Server>> Servers => GetServers();
}

public interface IHangfireDataService
{
    Task<IList<Server>> GetServers(CancellationToken cancellationToken = default);
    Task OpenConnectionAsync();
    Task<IList<Server>> Servers { get; }
}