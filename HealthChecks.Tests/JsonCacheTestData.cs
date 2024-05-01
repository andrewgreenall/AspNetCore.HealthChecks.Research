using AspNetCore.Sonar.HealthChecks.Models;

namespace HealthChecks.Tests;

public static class JsonCacheTestData
{
    public static QualityGateProjectStatus cacheStoreAllOk { get; } = new QualityGateProjectStatus
    {
        projectStatus = new ProjectStatus
        {
            status = "OK",
            conditions = new List<Condition>
                {
                    new Condition
                    {
                        status = "OK",
                        metricKey = "bugs",
                        comparator = "GT",
                        periodIndex = 1,
                        errorThreshold = "0",
                        actualValue = "0"
                    },
                    new Condition
                    {
                        status = "OK",
                        metricKey = "vulnerabilities",
                        comparator = "GT",
                        periodIndex = 1,
                        errorThreshold = "0",
                        actualValue = "0"
                    },
                    new Condition
                    {
                        status = "OK",
                        metricKey = "code_smells",
                        comparator = "GT",
                        periodIndex = 1,
                        errorThreshold = "0",
                        actualValue = "0"
                    },
                    new Condition
                    {
                        status = "OK",
                        metricKey = "duplicated_lines_density",
                        comparator = "GT",
                        periodIndex = 1,
                        errorThreshold = "3",
                        actualValue = "1.5"
                    }
                }
        }
    };
    public static QualityGateProjectStatus cacheStoreBugError { get; } = new QualityGateProjectStatus
    {
        projectStatus = new ProjectStatus
        {
            status = "OK",
            conditions = new List<Condition>
                {
                    new Condition
                    {
                        status = "ERROR",
                        metricKey = "bugs",
                        comparator = "GT",
                        periodIndex = 1,
                        errorThreshold = "0",
                        actualValue = "0"
                    },
                    new Condition
                    {
                        status = "OK",
                        metricKey = "vulnerabilities",
                        comparator = "GT",
                        periodIndex = 1,
                        errorThreshold = "0",
                        actualValue = "0"
                    },
                    new Condition
                    {
                        status = "OK",
                        metricKey = "code_smells",
                        comparator = "GT",
                        periodIndex = 1,
                        errorThreshold = "0",
                        actualValue = "0"
                    },
                    new Condition
                    {
                        status = "OK",
                        metricKey = "duplicated_lines_density",
                        comparator = "GT",
                        periodIndex = 1,
                        errorThreshold = "3",
                        actualValue = "1.5"
                    }
                }
        }
    };
    public static QualityGateProjectStatus cacheStoreReliabilityError { get; } = new QualityGateProjectStatus
    {
        projectStatus = new ProjectStatus
        {
            status = "OK",
            conditions = new List<Condition>
            {
                new Condition
                {
                    status = "ERROR",
                    metricKey = "new_reliability_rating",
                    comparator = "GT",
                    periodIndex = 1,
                    errorThreshold = "1",
                    actualValue = "3"
                }
            }
        }
    };
    public static QualityGateProjectStatus cacheStoreReliabilityWarning { get; } = new QualityGateProjectStatus
    {
        projectStatus = new ProjectStatus
        {
            status = "OK",
            conditions = new List<Condition>
            {
                new Condition
                {
                    status = "WARN",
                    metricKey = "new_reliability_rating",
                    comparator = "GT",
                    periodIndex = 1,
                    errorThreshold = "1",
                    actualValue = "1"
                }
            }
        }
    };
    public static QualityGateProjectStatus cacheStoreReliabilityOk { get; } = new QualityGateProjectStatus
    {
        projectStatus = new ProjectStatus
        {
            status = "OK",
            conditions = new List<Condition>
            {
                new Condition
                {
                    status = "OK",
                    metricKey = "new_reliability_rating",
                    comparator = "GT",
                    periodIndex = 1,
                    errorThreshold = "1",
                    actualValue = "3"
                }
            }
        }
    };
}
