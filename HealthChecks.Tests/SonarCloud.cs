using System.Text.Json;
using AspNetCore.Sonar.HealthChecks;
using AspNetCore.Sonar.HealthChecks.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace HealthChecks.Tests;

[TestClass]
public class SonarCloudTests
{
    private IConfiguration? _configuration;

    [TestInitialize]
    public void Setup()
    {
        _configuration = new ConfigurationBuilder()
            .AddUserSecrets<SonarCloudTests>()
            .Build();
    }

    /// <summary>
    /// Checks the SonarCloud project status. This is an integration test that requires a SonarCloud project key and token.
    /// </summary>
    [TestMethod]
    [Ignore("Integration test")]
    public async Task ProjectStatusForProject0()
    {
        UnitTestGuards.ThrowIfNull(_configuration, "Configuration not found.");

        // Arrange
        var sonarCloudOptions = new SonarCloudOptions
        {
            ServerUrl = "https://sonarcloud.io",
            ProjectKey = _configuration?["SonarCloud:Projects:0"],
            Token = _configuration?["SonarCloud:Token"]
        };
        var httpClient = new HttpClient();
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        memoryCache.Set("SonarCloudProjectHealthCheck", string.Empty);
        var sonarCloudProjectHealthCheck = new SonarCloudProjectHealthCheck(sonarCloudOptions, httpClient, memoryCache);

        // Act
        var healthCheckResult = await sonarCloudProjectHealthCheck.CheckHealthAsync(new HealthCheckContext());

        // Assert
        Assert.AreEqual(HealthStatus.Healthy, healthCheckResult.Status);

    }

    /// <summary>
    /// Checks the SonarCloud project status. This test should use the cache to return a healthy status without using the Http client.
    /// </summary>
    [TestMethod]
    public async Task ProjectStatusForProject0UsingCache()
    {
        UnitTestGuards.ThrowIfNull(_configuration, "Configuration not found.");

        // Arrange
        var sonarCloudOptions = new SonarCloudOptions
        {
            ServerUrl = "https://sonarcloud.io",
            ProjectKey = _configuration?["SonarCloud:Projects:0"],
            Token = _configuration?["SonarCloud:Token"]
        };
        var httpClient = new HttpClient();
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        memoryCache.Set("SonarCloudProjectHealthCheck", JsonSerializer.Serialize(JsonCacheTestData.cacheStoreAllOk));
        var sonarCloudProjectHealthCheck = new SonarCloudProjectHealthCheck(sonarCloudOptions, httpClient, memoryCache);

        // Act
        var healthCheckResult = await sonarCloudProjectHealthCheck.CheckHealthAsync(new HealthCheckContext());

        // Assert
        Assert.AreEqual(HealthStatus.Healthy, healthCheckResult.Status);
    }

    /// <summary>
    /// Checks the SonarCloud project status. This test should use the cache to return a healthy status without using the Http client.
    /// One of the cache conditions should be in error.
    /// </summary>
    [TestMethod]
    public async Task ProjectStatusForProject0UsingCacheWithError()
    {
        UnitTestGuards.ThrowIfNull(_configuration, "Configuration not found.");

        // Arrange
        var sonarCloudOptions = new SonarCloudOptions
        {
            ServerUrl = "https://sonarcloud.io",
            ProjectKey = _configuration?["SonarCloud:Projects:0"],
            Token = _configuration?["SonarCloud:Token"]
        };
        var httpClient = new HttpClient();
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        memoryCache.Set("SonarCloudProjectHealthCheck", JsonSerializer.Serialize(JsonCacheTestData.cacheStoreBugError));
        var sonarCloudProjectHealthCheck = new SonarCloudProjectHealthCheck(sonarCloudOptions, httpClient, memoryCache);

        // Act
        var healthCheckResult = await sonarCloudProjectHealthCheck.CheckHealthAsync(new HealthCheckContext());

        // Assert
        Assert.AreEqual(HealthStatus.Unhealthy, healthCheckResult.Status);
        Assert.AreEqual("The SonarCloud project quality gate for " + sonarCloudOptions.ProjectKey + " has failed.", healthCheckResult.Description);
    }

    /// <summary>
    /// Checks the SonarCloud project status. This is an integration test that requires a SonarCloud project key and token.
    /// </summary>
    [TestMethod]
    [Ignore("Integration test")]
    public async Task ProjectNewReliabilityRatingForProject0()
    {
        UnitTestGuards.ThrowIfNull(_configuration, "Configuration not found.");

        // Arrange
        var sonarCloudOptions = new SonarCloudOptions
        {
            ServerUrl = "https://sonarcloud.io",
            ProjectKey = _configuration?["SonarCloud:Projects:0"],
            Token = _configuration?["SonarCloud:Token"]
        };
        var httpClient = new HttpClient();
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        memoryCache.Set("SonarCloudProjectHealthCheck", string.Empty);
        var sonarCloudProjectNewReliabilityRatingHealthCheck = new SonarCloudProjectNewReliabilityRatingHealthCheck(sonarCloudOptions, httpClient, memoryCache);

        // Act
        var healthCheckResult = await sonarCloudProjectNewReliabilityRatingHealthCheck.CheckHealthAsync(new HealthCheckContext());

        // Assert
        Assert.AreEqual(HealthStatus.Healthy, healthCheckResult.Status);
    }

    [TestMethod]
    public async Task ProjectNewReliabilityRatingForProject0UsingCacheWithError()
    {
        UnitTestGuards.ThrowIfNull(_configuration, "Configuration not found.");

        // Arrange
        var sonarCloudOptions = new SonarCloudOptions
        {
            ServerUrl = "https://sonarcloud.io",
            ProjectKey = _configuration?["SonarCloud:Projects:0"], 
            Token = _configuration?["SonarCloud:Token"] 
        };
        var httpClient = new HttpClient();
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        memoryCache.Set("SonarCloudProjectHealthCheckNewReliabilityRating", JsonSerializer.Serialize(JsonCacheTestData.cacheStoreReliabilityError));
        var sonarCloudProjectNewReliabilityRatingHealthCheck = new SonarCloudProjectNewReliabilityRatingHealthCheck(sonarCloudOptions, httpClient, memoryCache);

        // Act
        var healthCheckResult = await sonarCloudProjectNewReliabilityRatingHealthCheck.CheckHealthAsync(new HealthCheckContext());

        // Assert
        Assert.AreEqual(HealthStatus.Unhealthy, healthCheckResult.Status);
        Assert.AreEqual("Project quality gate for " + sonarCloudOptions.ProjectKey + " has failed new reliability rating.", healthCheckResult.Description);
    }

    [TestMethod]
    public async Task ProjectNewReliabilityRatingForProject0UsingCacheWithWarning()
    {
        UnitTestGuards.ThrowIfNull(_configuration, "Configuration not found.");

        // Arrange
        var sonarCloudOptions = new SonarCloudOptions
        {
            ServerUrl = "https://sonarcloud.io",
            ProjectKey = _configuration?["SonarCloud:Projects:0"],
            Token = _configuration?["SonarCloud:Token"]
        };
        var httpClient = new HttpClient();
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        memoryCache.Set("SonarCloudProjectHealthCheckNewReliabilityRating", JsonSerializer.Serialize(JsonCacheTestData.cacheStoreReliabilityWarning));
        var sonarCloudProjectNewReliabilityRatingHealthCheck = new SonarCloudProjectNewReliabilityRatingHealthCheck(sonarCloudOptions, httpClient, memoryCache);

        // Act
        var healthCheckResult = await sonarCloudProjectNewReliabilityRatingHealthCheck.CheckHealthAsync(new HealthCheckContext());

        // Assert
        Assert.AreEqual(HealthStatus.Unhealthy, healthCheckResult.Status);
        Assert.AreEqual("Project quality gate for " + sonarCloudOptions.ProjectKey + " has failed new reliability rating.", healthCheckResult.Description);
    }

    [TestMethod]
    public async Task ProjectNewReliabilityRatingForProject0UsingCacheWithOk()
    {
        UnitTestGuards.ThrowIfNull(_configuration, "Configuration not found.");

        // Arrange
        var sonarCloudOptions = new SonarCloudOptions
        {
            ServerUrl = "https://sonarcloud.io",
            ProjectKey = _configuration?["SonarCloud:Projects:0"],
            Token = _configuration?["SonarCloud:Token"]
        };
        var httpClient = new HttpClient();
        var memoryCache = new MemoryCache(new MemoryCacheOptions());
        memoryCache.Set("SonarCloudProjectHealthCheckNewReliabilityRating", JsonSerializer.Serialize(JsonCacheTestData.cacheStoreReliabilityOk));
        var sonarCloudProjectNewReliabilityRatingHealthCheck = new SonarCloudProjectNewReliabilityRatingHealthCheck(sonarCloudOptions, httpClient, memoryCache);

        // Act
        var healthCheckResult = await sonarCloudProjectNewReliabilityRatingHealthCheck.CheckHealthAsync(new HealthCheckContext());

        // Assert
        Assert.AreEqual(HealthStatus.Healthy, healthCheckResult.Status);

    }
}