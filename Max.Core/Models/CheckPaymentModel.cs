using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class CheckPaymentModel
    {
        public int OrganizationId { get; set; }
        public int CartId { get; set; }
        public int UserId { get; set; }
        public int EntityId { get; set; }
        public string CheckNumber { get; set; }
        public string AccountNumber { get; set; }
        public string BankName { get; set; }
        public string AccountHolderName { get; set; }
        public string AccountType { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}
