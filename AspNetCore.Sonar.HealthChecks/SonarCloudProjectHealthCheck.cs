using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using AspNetCore.Sonar.HealthChecks.Models;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Caching.Memory;

namespace AspNetCore.Sonar.HealthChecks;

public class SonarCloudProjectHealthCheck : IHealthCheck
{
    private readonly SonarCloudOptions _sonarCloudOptions;
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;

    public SonarCloudProjectHealthCheck(SonarCloudOptions sonarCloudOptions, HttpClient httpClient, IMemoryCache cache)
    {
        _sonarCloudOptions = sonarCloudOptions;
        _httpClient = httpClient;
        _cache = cache;
    }
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        // guard clause to check if the SonarCloud server URL is not configured
        if (string.IsNullOrWhiteSpace(_sonarCloudOptions.ServerUrl))
        {
            return HealthCheckResult.Unhealthy("The SonarCloud server URL is not configured.");
        }
        // guard clause to check if the SonarCloud project key is not configured
        if (string.IsNullOrWhiteSpace(_sonarCloudOptions.ProjectKey))
        {
            return HealthCheckResult.Unhealthy("The SonarCloud project key is not configured.");
        }
        // guard clause to check if the SonarCloud token is not configured
        if (string.IsNullOrWhiteSpace(_sonarCloudOptions.Token))
        {
            return HealthCheckResult.Unhealthy("The SonarCloud token is not configured.");
        }

        // create a cache to store the response
        var cache = _cache.GetOrCreate<string>("SonarCloudProjectHealthCheck", entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_sonarCloudOptions.CacheExpirationTimeInMinutes);
            return string.Empty;
        });

        if (!string.IsNullOrWhiteSpace(cache))
        {
            var cachedResponse = JsonSerializer.Deserialize<QualityGateProjectStatus>(cache);
            if (cachedResponse.projectStatus.conditions.Exists(c => c.status is "ERROR" or "WARN"))
            {
                return HealthCheckResult.Unhealthy($"The SonarCloud project quality gate for {_sonarCloudOptions.ProjectKey} has failed.");
            }
        }
        else
        {
            //var sonarCloudApiUrl = $"{_sonarCloudOptions.ServerUrl}/api/project_analyses/search?project={_sonarCloudOptions.ProjectKey}";
            var sonarCloudApiUrl = $"{_sonarCloudOptions.ServerUrl}/api/qualitygates/project_status?projectKey={_sonarCloudOptions.ProjectKey}";
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _sonarCloudOptions.Token);
            var response = await _httpClient.GetAsync(sonarCloudApiUrl, cancellationToken);

            // check if the response is authorized
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return HealthCheckResult.Unhealthy("The SonarCloud token is not authorized.");
            }
            if (!response.IsSuccessStatusCode)
            {
                return HealthCheckResult.Unhealthy("The SonarCloud project request did not succeed.");
            }

            // place the body content in a variable
            var content = await response.Content.ReadAsStringAsync();
            _cache.Set("SonarCloudProjectHealthCheck", content);
            // convert the content to a json object ProjectAnalysisResult
            var projectAnalysisResult = JsonSerializer.Deserialize<QualityGateProjectStatus>(content);
            if (projectAnalysisResult.projectStatus.conditions.Any(c => c.status == "ERROR" || c.status == "WARN"))
            {
                return HealthCheckResult.Unhealthy($"The SonarCloud project quality gate for {_sonarCloudOptions.ProjectKey} has failed.");
            }
        }

        return HealthCheckResult.Healthy();
    }
}