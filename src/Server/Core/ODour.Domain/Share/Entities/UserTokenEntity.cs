using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using ODour.Domain.Share.Entities.Base;

namespace ODour.Domain.Share.Entities;

public sealed class UserTokenEntity : IdentityUserToken<Guid>, IEntity
{
    public Guid Id { get; set; }

    [NotMapped]
    public DateTime CreatedAt { get; set; }

    public DateTime ExpiredAt { get; set; }

    #region Relationships
    [NotMapped]
    public UserEntity User { get; set; }
    #endregion

    #region MetaData
    public static class MetaData
    {
        public const string TableName = "UserTokens";
    }
    #endregion
}
