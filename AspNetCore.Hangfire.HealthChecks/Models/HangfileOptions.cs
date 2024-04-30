namespace AspNetCore.Hangfire.HealthChecks.Models;

public class HangfireOptions
{
    /// <summary>
    /// Gets or sets the Hangfire server name.
    /// </summary>
    public string SqlConnectionString { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the Hangfire server name.
    /// </summary>
    public string ServerName { get; set; } = string.Empty;
    /// <summary>
    /// Gets or sets the heartbeat timeout for the Hangfire server.
    /// </summary>
    public int HeartbeatTimeout { get; set; } = 5;
    /// <summary>
    /// Gets or sets the Schema name for the Hangfire data.
    /// </summary>
    public string SchemaName { get; set; } = "HangFire";
}