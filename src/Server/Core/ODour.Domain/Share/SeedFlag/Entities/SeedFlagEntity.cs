using System;
using ODour.Domain.Share.Base.Entities;

namespace ODour.Domain.Share.SeedFlag.Entities;

public sealed class SeedFlagEntity : IEntity
{
    #region PrimaryKeys
    public Guid Id { get; set; }
    #endregion

    public static class MetaData
    {
        public const string TableName = "SeedFlags";
    }
}
