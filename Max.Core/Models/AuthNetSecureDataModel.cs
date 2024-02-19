using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class AuthNetSecureDataModel
    {
        public int OrganizationId { get; set; }
        public int CartId { get; set; }
        public int UserId { get; set; }
        public int EntityId { get; set; }
        public int BillableEntityId { get; set; }
        public string PaymentMode { get; set; }
        public string DataValue { get; set; }
        public string AccountHolderName { get; set; }
        public string BankName { get; set; }
        public string RoutingNumber { get; set; }
        public string AccountType { get; set; }
        public string StreetAddress { get; set; }
        public string Zip { get; set; }
        public string FullName { get; set; }
        public int SavePaymentProfile { get; set; }
        public string NickName { get; set; }
    }
}
