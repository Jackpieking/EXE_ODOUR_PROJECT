using System;
using System.Threading;
using System.Threading.Tasks;

namespace ODour.Application.Share.Mail;

/// <summary>
///     Represent interface of sending mail handler.
/// </summary>
public interface ISendingMailHandler
{
    /// <summary>
    ///     Get user account confirmation mail content.
    /// </summary>
    /// <param name="to">
    ///     Send to whom.
    /// </param>
    /// <param name="subject">
    ///     Mail subject
    /// </param>
    /// <param name="mainLink">
    ///     Main mail verification link.
    /// </param>
    /// <param name="cancellationToken">
    ///     A token that is used for notifying system
    ///     to cancel the current operation when user stop
    ///     the request.
    /// </param>
    /// <returns>
    ///     Model contain receiver information.
    /// </returns>
    Task<AppMailContent> GetUserAccountConfirmationMailContentAsync(
        string to,
        string subject,
        string mainLink,
        CancellationToken cancellationToken
    );

    /// <summary>
    ///     Get user account confirmation mail content.
    /// </summary>
    /// <param name="to">
    ///     Send to whom.
    /// </param>
    /// <param name="subject">
    ///     Mail subject
    /// </param>
    /// <param name="resetPasswordLink">
    ///     Mail reset password token.
    /// </param>
    /// <param name="cancellationToken">
    ///     A token that is used for notifying system
    ///     to cancel the current operation when user stop
    ///     the request.
    /// </param>
    /// <returns>
    ///     Model contain receiver information.
    /// </returns>
    Task<AppMailContent> GetUserResetPasswordMailContentAsync(
        string to,
        string subject,
        string resetPasswordLink,
        CancellationToken cancellationToken
    );

    Task<AppMailContent> GetUserConfirmSuccessfullyMailContentAsync(
        string to,
        string subject,
        CancellationToken cancellationToken
    );

    Task<AppMailContent> GetNotifyUserAboutLoginActionMailContentAsync(
        string to,
        string subject,
        DateTime currentTimeInLocal,
        CancellationToken cancellationToken
    );

    Task<AppMailContent> GetUserPasswordChangedSuccessfullyMailContentAsync(
        string to,
        string subject,
        CancellationToken cancellationToken
    );

    /// <summary>
    ///     Sending an email to the specified user.
    /// </summary>
    /// <param name="mailContent">
    ///     A model contains all receiver information.
    /// </param>
    /// <param name="cancellationToken">
    ///     A token that is used for notifying system
    ///     to cancel the current operation when user stop
    ///     the request.
    /// </param>
    /// <returns>
    ///     Task containing boolean result.
    /// </returns>
    Task<bool> SendAsync(AppMailContent mailContent, CancellationToken cancellationToken);
}
