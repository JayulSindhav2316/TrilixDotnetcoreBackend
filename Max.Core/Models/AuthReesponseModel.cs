using System;
using System.Collections.Generic;
using System.Text;
using Max.Core.Models;
using Newtonsoft.Json;

namespace Max.Core.Models
{
    public class AuthResponseModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Token { get; set; }
        public int CartId { get; set; }
        public int OrganizationId { get; set; }
        public string RefreshToken { get; set; }
        public string IpAddress { get; set; }
        public string TenantId { get; set; }
        public string TenantCN { get; set; }
        public string TenantRCN { get; set; }   
        public string ReportEmailSender { get; set; }
        public string OrganizationName { get; set; }
        public string AccountName { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsBirthdayRequired { get; set; }
        public List<BillingCycleNotifications> Notifications { get; set; }

        public AuthResponseModel(StaffUserModel user, string token, string refreshToken, string ipAddress, string tenantId , string tenantCN, string tenantRCN , string reportEmailSender, List<BillingCycleNotifications> notifications)
        {
            Id = user.UserId;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Username = user.UserName;
            Token = token;
            RefreshToken = refreshToken;
            CartId = user.CartId;
            OrganizationId = user.OrganizationId;
            IpAddress = ipAddress;
            TenantId = tenantId;
            TenantCN = tenantCN;
            TenantRCN = tenantRCN;
            OrganizationName = user.OrganizationName;
            AccountName = user.AccountName;
            IsAdmin = user.IsAdmin;
            IsBirthdayRequired = user.IsBirthdayRequired;
            ReportEmailSender = reportEmailSender;
            Notifications = notifications;
        }
        
    }
}
