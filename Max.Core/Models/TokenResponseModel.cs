using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class TokenResponseModel
    {
        public int UserId { get; set; }
        public int EntityId { get; set; }
        public string Token { get; set; } 
        public string RefreshToken { get; set; } 
        public string TenantId { get; set; }
        public ResponseStatusModel ResponseStatus { get; set; }
        public List<BillingCycleNotifications> Notifications { get; set; }
    }
}
