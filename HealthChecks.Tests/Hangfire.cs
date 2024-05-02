using AspNetCore.Hangfire.HealthChecks;
using AspNetCore.Hangfire.HealthChecks.Models;
using AspNetCore.Hangfire.HealthChecks.Models.Data;
using AspNetCore.Hangfire.HealthChecks.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Moq;

namespace HealthChecks.Tests;

[TestClass]
public class Hangfire
{
    private IConfiguration? _configuration { get; set; }

    [TestInitialize]
    public void Setup()
    {
        _configuration = new ConfigurationBuilder()
            .AddUserSecrets<SonarCloudTests>()
            .Build();
    }

    [TestMethod]
    public void ConvertServerIdToModel()
    {
        // Arrange
        var serverId = "ServiceName:MachineName:8078:InstanceId";

        // Act
        var result = HangFireUtils.ConvertServerIdToModel(serverId);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("ServiceName", result.ServiceName);
        Assert.AreEqual("MachineName", result.MachineName);
        Assert.AreEqual("InstanceId", result.InstanceId);
    }

    [TestMethod]
    [Ignore("Integration test")]
    public async Task GetSingleServiceOnServer()
    {
        UnitTestGuards.ThrowIfNull(_configuration, "Configuration not found.");
        // Arrange
        var parameters = new HangfireOptions
        {
            SqlConnectionString = _configuration?["Hangfire:ConnectionString"],
            ServerName = _configuration?["Hangfire:ServiceName"],
            MachineName = _configuration?["Hangfire:MachineName"],
        };
        var hangfireSingleServerHealthCheck = new HangfireSingleServerHealthCheck(parameters, new HangfireDataService(parameters.SqlConnectionString, parameters.SchemaName));

        // Act
        var result = await hangfireSingleServerHealthCheck.CheckHealthAsync(new HealthCheckContext());

        // Assert
        Assert.IsNotNull(result);
        Assert.IsNull(result.Description);
        Assert.AreEqual(HealthStatus.Healthy, result.Status);
    }

    [TestMethod]
    public async Task GetSingleServiceOnServerHealthyResult()
    {
        // Arrange
        var parameters = new HangfireOptions
        {
            SqlConnectionString = "a connection string",
            ServerName = "ServiceName",
            MachineName = "MachineName",
        };

        // mock the HangfireDataService
        var hangfireDataService = new Mock<IHangfireDataService>();
        hangfireDataService.Setup(x => x.GetServers(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Server>
        {
            new Server
            {
                Id = "ServiceName:MachineName:8078:InstanceId",
                Data = "Data",
                LastHeartbeat = DateTime.UtcNow
            }
        });

        var hangfireSingleServerHealthCheck = new HangfireSingleServerHealthCheck(parameters, hangfireDataService.Object);

        // Act
        var result = await hangfireSingleServerHealthCheck.CheckHealthAsync(new HealthCheckContext());

        // Assert
        Assert.IsNotNull(result);
        Assert.IsNull(result.Description);
        Assert.AreEqual(HealthStatus.Healthy, result.Status);
    }

    [TestMethod]
    public async Task GetSingleServiceOnServerUnHealthyResult()
    {
        // Arrange
        var parameters = new HangfireOptions
        {
            SqlConnectionString = "a connection string",
            ServerName = "ServiceName",
            MachineName = "MachineName",
        };

        // mock the HangfireDataService
        var hangfireDataService = new Mock<IHangfireDataService>();
        hangfireDataService.Setup(x => x.GetServers(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Server>
        {
            new Server
            {
                Id = "ServiceName:MachineName:8078:InstanceId",
                Data = "Data",
                LastHeartbeat = DateTime.UtcNow.AddHours(-1)
            }
        });

        var hangfireSingleServerHealthCheck = new HangfireSingleServerHealthCheck(parameters, hangfireDataService.Object);

        // Act
        var result = await hangfireSingleServerHealthCheck.CheckHealthAsync(new HealthCheckContext());

        // Assert
        Assert.IsNotNull(result);
        Assert.IsNotNull(result.Description);
        Assert.AreEqual(HealthStatus.Unhealthy, result.Status);
    }
}
