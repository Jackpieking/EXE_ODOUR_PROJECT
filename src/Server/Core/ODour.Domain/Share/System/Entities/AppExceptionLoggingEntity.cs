using System;
using ODour.Domain.Share.Base.Entities;

namespace ODour.Domain.Share.System.Entities;

public sealed class AppExceptionLoggingEntity : IEntity
{
    #region PrimaryKeys
    public Guid Id { get; set; }
    #endregion

    public string ErrorMessage { get; set; }

    public string ErrorStackTrace { get; set; }

    public DateTime CreatedAt { get; set; }

    public string Data { get; set; }

    #region MetaData
    public static class MetaData
    {
        public const string TableName = "AppExceptionLoggingEntities";

        public static class ErrorMessage
        {
            public const int MinLength = 2;
        }

        public static class ErrorStackTrace
        {
            public const int MinLength = 2;
        }

        public static class Data
        {
            public const int MinLength = 2;
        }
    }
    #endregion
}
