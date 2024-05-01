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
    /// <summary>
    /// Checks the SonarCloud project status. This is an integration test that requires a SonarCloud project key and token.
    /// </summary>
    [TestMethod]
    [Ignore("Integration test")]
    public async Task ProjectStatusForProject0()
    {
        // collect user secrets
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<SonarCloudTests>()
            .Build();

        // Arrange
        var sonarCloudOptions = new SonarCloudOptions
        {
            ServerUrl = "https://sonarcloud.io",
            ProjectKey = configuration["SonarCloud:Projects:0"],
            Token = configuration["SonarCloud:Token"]
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
        // collect user secrets
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<SonarCloudTests>()
            .Build();

        // Arrange
        var sonarCloudOptions = new SonarCloudOptions
        {
            ServerUrl = "https://sonarcloud.io",
            ProjectKey = configuration["SonarCloud:Projects:0"],
            Token = configuration["SonarCloud:Token"]
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
        // collect user secrets
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<SonarCloudTests>()
            .Build();

        // Arrange
        
        var sonarCloudOptions = new SonarCloudOptions
        {
            ServerUrl = "https://sonarcloud.io",
            ProjectKey = configuration["SonarCloud:Projects:0"],
            Token = configuration["SonarCloud:Token"]
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
        // collect user secrets
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<SonarCloudTests>()
            .Build();

        // Arrange
        var sonarCloudOptions = new SonarCloudOptions
        {
            ServerUrl = "https://sonarcloud.io",
            ProjectKey = configuration["SonarCloud:Projects:0"],
            Token = configuration["SonarCloud:Token"]
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
        // collect user secrets
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<SonarCloudTests>()
            .Build();

        // Arrange
        var sonarCloudOptions = new SonarCloudOptions
        {
            ServerUrl = "https://sonarcloud.io",
            ProjectKey = configuration["SonarCloud:Projects:0"],
            Token = configuration["SonarCloud:Token"]
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
        // collect user secrets
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<SonarCloudTests>()
            .Build();

        // Arrange
        var sonarCloudOptions = new SonarCloudOptions
        {
            ServerUrl = "https://sonarcloud.io",
            ProjectKey = configuration["SonarCloud:Projects:0"],
            Token = configuration["SonarCloud:Token"]
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
        // collect user secrets
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<SonarCloudTests>()
            .Build();

        // Arrange
        var sonarCloudOptions = new SonarCloudOptions
        {
            ServerUrl = "https://sonarcloud.io",
            ProjectKey = configuration["SonarCloud:Projects:0"],
            Token = configuration["SonarCloud:Token"]
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