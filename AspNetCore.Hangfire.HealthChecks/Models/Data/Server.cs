using System;
using System.Collections.Generic;
using System.Linq;

namespace AspNetCore.Hangfire.HealthChecks.Models.Data;

public class Server
{
    public string Id { get; set; }
    public string Data { get; set; }
    public DateTime LastHeartbeat { get; set; }
}

public class ServerDtoModel
{
    public string ServiceName { get; set; }
    public string MachineName { get; set; }
    public string Port { get; set; }
    public string InstanceId { get; set; }
    public string Data { get; set; }
    public DateTime LastHeartbeat { get; set; }
}

public static class SeverExtention
{
    public static ServerDtoModel ToDtoModel(this Server server)
    {
        var serverId = HangFireUtils.ConvertServerIdToModel(server.Id);
        var serverData = server.Data;
        return new ServerDtoModel
        {
            ServiceName = serverId.ServiceName,
            MachineName = serverId.MachineName,
            Port = serverId.Port,
            InstanceId = serverId.InstanceId,
            Data = serverData,
            LastHeartbeat = server.LastHeartbeat
        };
    }

    public static IList<ServerDtoModel> ToDtoModel(this IEnumerable<Server> servers)
    {
        return servers.Select(server => server.ToDtoModel()).ToList();
    }
}