namespace AspNetCore.Sonar.HealthChecks.Models;

public class SonarCloudOptions
{
    /// <summary>
    /// Gets or sets the SonarCloud server URL.
    /// </summary>
    public string ServerUrl { get; set; } = "https://sonarcloud.io";

    /// <summary>
    /// Gets or sets the SonarCloud project key.
    /// </summary>
    public string ProjectKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the SonarCloud token.
    /// </summary>
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the memory cache expiration time in miniutes.
    /// </summary>
    public int CacheExpirationTimeInMinutes { get; set; } = 5;
}