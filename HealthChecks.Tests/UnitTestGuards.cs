namespace HealthChecks.Tests;

public static class UnitTestGuards
{
    public static void ThrowIfNull(object? value, string parameterName)
    {
        if(value == null)
        {
            throw new ArgumentNullException(parameterName);
        }
    }
}
