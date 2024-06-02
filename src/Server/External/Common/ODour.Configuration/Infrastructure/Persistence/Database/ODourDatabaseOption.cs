namespace ODour.Configuration.Infrastructure.Persistence.Database;

public sealed class ODourDatabaseOption
{
    public string ConnectionString { get; set; }

    public int CommandTimeOut { get; set; }

    public int EnableRetryOnFailure { get; set; }

    public bool EnableSensitiveDataLogging { get; set; }

    public bool EnableDetailedErrors { get; set; }

    public bool EnableThreadSafetyChecks { get; set; }

    public bool EnableServiceProviderCaching { get; set; }
}
