using System;
using Microsoft.AspNetCore.Identity;
using ODour.Domain.Share.Entities.Base;

namespace ODour.Domain.Share.Entities;

public sealed class UserLoginEntity : IdentityUserLogin<Guid>, IEntity
{
    #region MetaData
    public static class MetaData
    {
        public const string TableName = "UserLogins";
    }
    #endregion
}
