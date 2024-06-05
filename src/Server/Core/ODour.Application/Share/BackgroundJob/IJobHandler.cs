using System;
using System.Linq.Expressions;

namespace ODour.Application.Share.BackgroundJob;

public interface IJobHandler
{
    void ExecuteOneTimeJob(Expression<Action> methodCall);

    void ExecuteOneTimeJobWithDelay(Expression<Action> methodCall, TimeSpan delayTime);

    void ExecuteRecurringJob(string jobId, Expression<Action> methodCall, string cronExpression);
}
