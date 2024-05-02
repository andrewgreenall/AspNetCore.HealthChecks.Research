using AspNetCore.Hangfire.HealthChecks;
using AspNetCore.Hangfire.HealthChecks.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;

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
        var hangfireSingleServerHealthCheck = new HangfireSingleServerHealthCheck(parameters);

        // Act
        var result = await hangfireSingleServerHealthCheck.CheckHealthAsync(new HealthCheckContext());

        // Assert
        Assert.IsNotNull(result);
        Assert.IsNull(result.Description);
        Assert.AreEqual(HealthStatus.Healthy, result.Status);
    }
}
