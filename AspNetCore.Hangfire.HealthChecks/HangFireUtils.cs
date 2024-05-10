using System;

namespace AspNetCore.Hangfire.HealthChecks;

public static class HangFireUtils
{
    /// <summary>
    /// Convert the Server Id to a ServerIdModel
    /// </summary>
    /// <param name="serverId">The server id</param>
    /// <returns>The ServerIdModel</returns>
    /// <exception cref="ArgumentNullException">Thrown if the server id is null</exception>
    /// <exception cref="ArgumentException">Thrown if the server id is empty</exception>
    public static ServerIdModel ConvertServerIdToModel(string serverId)
    {
        if (serverId == null)
        {
            throw new ArgumentNullException(nameof(serverId));
        }

        if (string.IsNullOrWhiteSpace(serverId))
        {
            throw new ArgumentException("The server id cannot be empty.", nameof(serverId));
        }

        var parts = serverId.Split(':');
        if (parts.Length != 4)
        {
            throw new ArgumentException("The server id is not in the correct format.", nameof(serverId));
        }

        return new ServerIdModel
        {
            ServiceName = parts[0],
            MachineName = parts[1],
            Port = parts[2],
            InstanceId = parts[3]
        };
    }
}
