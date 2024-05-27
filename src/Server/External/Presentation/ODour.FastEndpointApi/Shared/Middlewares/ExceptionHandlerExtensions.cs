using System.Collections.Generic;
using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using ODour.FastEndpointApi.Shared.AppCodes;
using ODour.FastEndpointApi.Shared.Response;

namespace ODour.FastEndpointApi.Shared.Middlewares;

public static class ExceptionHandlerExtensions
{
    public static IApplicationBuilder UseAppExceptionHandler(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(configure: errApp =>
        {
            errApp.Run(handler: async ctx =>
            {
                var exHandlerFeature = ctx.Features.Get<IExceptionHandlerFeature>();

                if (!Equals(objA: exHandlerFeature, objB: default))
                {
                    // await using var scope = _serviceScopeFactory.CreateAsyncScope();

                    // var unitOfWork = scope.Resolve<IUnitOfWork>();

                    // await unitOfWork.InsertErrorLogRepository.InsertErrorLogCommandAsync(
                    //     exception: exception,
                    //     cancellationToken: cancellationToken
                    // );

                    ctx.Response.Clear();
                    ctx.Response.StatusCode = StatusCodes.Status500InternalServerError;

                    await ctx.Response.WriteAsJsonAsync(
                        value: new OtherPurposeResponse
                        {
                            AppCode = OtherPurposeAppCode.SERVER_ERROR.ToString(),
                            ErrorMessages = new List<string>(capacity: 2)
                            {
                                "Server has encountered an error !!",
                                "Please contact admin for support."
                            }
                        },
                        cancellationToken: CancellationToken.None
                    );

                    await ctx.Response.CompleteAsync();
                }
            });
        });

        return app;
    }
}
