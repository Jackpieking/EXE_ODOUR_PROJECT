namespace ODour.Configuration.Infrastructure.BackgroundJob;

public sealed class HangFireOption
{
    public string ServerName { get; set; }

    public long SchedulePollingIntervalInSeconds { get; set; }

    public int WorkerCount { get; set; }
}
