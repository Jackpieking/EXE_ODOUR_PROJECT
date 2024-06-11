using System;
using System.Linq.Expressions;
using ODour.Application.Share.BackgroundJob;

namespace ODour.AppBackgroundJob.Handler;

public sealed class AppJobHandler : IJobHandler
{
    public void ExecuteOneTimeJob(Expression<Action> methodCall) { }

    public void ExecuteOneTimeJobWithDelay(Expression<Action> methodCall, TimeSpan delayTime) { }

    public void ExecuteRecurringJob(
        string jobId,
        Expression<Action> methodCall,
        string cronExpression
    ) { }
}
