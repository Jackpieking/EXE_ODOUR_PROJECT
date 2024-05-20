using System;
using ODour.Domain.Share.Entities.Base;

namespace ODour.Domain.Share.Entities;

public sealed class SystemAccountTokenEntity : IEntity
{
    #region PrimaryForeignKeys
    public Guid SystemAccountId { get; set; }
    #endregion

    public string Name { get; set; }

    public string Value { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime ExpiredAt { get; set; }

    #region NavigationProperties
    public SystemAccountEntity SystemAccount { get; set; }
    #endregion

    #region MetaData
    public static class MetaData
    {
        public const string TableName = "SystemAccountTokens";

        public static class Name
        {
            public const int MinLength = 2;

            public const int MaxLength = 450;
        }

        public static class Value
        {
            public const int MinLength = 1;
        }
    }
    #endregion
}
