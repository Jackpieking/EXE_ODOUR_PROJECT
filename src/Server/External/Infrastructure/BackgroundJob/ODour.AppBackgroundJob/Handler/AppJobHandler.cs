using System;
using System.Linq.Expressions;
using Hangfire;
using ODour.Application.Share.BackgroundJob;

namespace ODour.AppBackgroundJob.Handler;

internal sealed class AppJobHandler : IJobHandler
{
    public void ExecuteOneTimeJob(Expression<Action> methodCall)
    {
        BackgroundJob.Enqueue(methodCall: methodCall);
    }

    public void ExecuteOneTimeJobWithDelay(Expression<Action> methodCall, TimeSpan delayTime)
    {
        BackgroundJob.Schedule(methodCall: methodCall, delay: delayTime);
    }

    public void ExecuteRecurringJob(
        string jobId,
        Expression<Action> methodCall,
        string cronExpression
    )
    {
        RecurringJob.AddOrUpdate(
            recurringJobId: jobId,
            methodCall: methodCall,
            cronExpression: cronExpression
        );
    }
}
