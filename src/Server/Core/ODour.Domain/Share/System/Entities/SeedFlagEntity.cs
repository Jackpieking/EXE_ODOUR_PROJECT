using System;
using ODour.Domain.Share.Base.Entities;

namespace ODour.Domain.Share.System.Entities
{
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
}
