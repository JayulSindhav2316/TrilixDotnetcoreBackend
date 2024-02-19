using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class AccessTokenModel
    {
        public int AccessTokenId { get; set; }
        public string Token { get; set; }
        public DateTime Create { get; set; }
        public DateTime Expire { get; set; }
        public string CreatedIp { get; set; }
        public string RevokedIp { get; set; }
        public DateTime Revoked { get; set; }
        public string RefreshToken { get; set; }
        public int UserId { get; set; }
        public int EntityId { get; set; }
        public bool IsExpired => DateTime.UtcNow >= Expire;
        public bool IsActive => Revoked == Constants.MySQL_MinDate && !IsExpired;

    }
}
