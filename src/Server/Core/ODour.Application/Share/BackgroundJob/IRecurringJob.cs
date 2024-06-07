using System;

namespace ODour.Application.Share.BackgroundJob;

public interface IRecurringJob
{
    public TimeSpan Frequency { get; set; }
}
