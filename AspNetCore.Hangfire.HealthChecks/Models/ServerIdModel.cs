using System;

namespace AspNetCore.Hangfire.HealthChecks;

public class ServerIdModel
{
    // the service name
    public string ServiceName { get; set; }
    // the server or machine name the service is running on
    public string MachineName { get; set; }
    // port number used by the service
    public string Port { get; set; }
    // not sure what this is, guessing
    public string InstanceId { get; set; }
}
