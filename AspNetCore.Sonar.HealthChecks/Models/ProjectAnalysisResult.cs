namespace AspNetCore.Sonar.HealthChecks.Models;

public class ProjectAnalysisResult
{
    public Paging paging { get; set; }
    public List<Analysis> analyses { get; set; }
}

public class Paging
{
    public int pageIndex { get; set; }
    public int pageSize { get; set; }
    public int total { get; set; }
}

public class Analysis
{
    public string key { get; set; }
    public string date { get; set; }
    public List<Event> events { get; set; }
    public string projectVersion { get; set; }
    public string buildString { get; set; }
    public bool manualNewCodePeriodBaseline { get; set; }
    public string revision { get; set; }
}

public class Event
{
    public string key { get; set; }
    public string category { get; set; }
    public string name { get; set; }
    public string description { get; set; }
}