using System;

namespace AspNetCore.Hangfire.HealthChecks;

public class Server
{
    public string Id { get; set; }
    public string Data { get; set; }
    public DateTime LastHeartbeat { get; set; }
}
