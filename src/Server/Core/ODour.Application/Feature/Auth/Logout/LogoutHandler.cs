using System;
using System.Threading;
using System.Threading.Tasks;
using ODour.Application.Share.Features;
using ODour.Domain.Feature.Main;

namespace ODour.Application.Feature.Auth.Logout;

internal sealed class LogoutHandler : IFeatureHandler<LogoutRequest, LogoutResponse>
{
    private readonly Lazy<IMainUnitOfWork> _unitOfWork;

    public LogoutHandler(Lazy<IMainUnitOfWork> unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<LogoutResponse> ExecuteAsync(LogoutRequest command, CancellationToken ct)
    {
        // Attempt to remove refresh token by its value.
        var dbResult =
            await _unitOfWork.Value.LogoutRepository.RemoveRefreshTokenByItsValueCommandAsync(
                refreshToken: command.GetRefreshToken(),
                ct: ct
            );

        if (!dbResult)
        {
            return new() { StatusCode = LogoutResponseStatusCode.OPERATION_FAIL };
        }

        return new() { StatusCode = LogoutResponseStatusCode.OPERATION_SUCCESS };
    }
}
