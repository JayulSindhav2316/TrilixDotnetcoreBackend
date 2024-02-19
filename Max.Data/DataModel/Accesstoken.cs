using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Accesstoken
    {
        public int AccessTokenId { get; set; }
        public string Token { get; set; }
        public DateTime Create { get; set; }
        public DateTime Expire { get; set; }
        public string CreatedIp { get; set; }
        public string RevokedIp { get; set; }
        public DateTime Revoked { get; set; }
        public string RefreshToken { get; set; }
        public int? UserId { get; set; }
        public int? EntityId { get; set; }
    }
}
