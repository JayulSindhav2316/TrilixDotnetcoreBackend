using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class AutoBillingDraftModel
    {
        public int AutoBillingDraftId { get; set; }
        public int? BillingDocumentId { get; set; }
        public string Name { get; set; }
        public string BillableName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? EntityId { get; set; }
        public decimal Amount { get; set; }
        public DateTime? NextDueDate { get; set; }
        public string CardNumber { get; set; }
        public string ExpirationDate { get; set; }
        public string CardType { get; set; }
        public string BankAccountNumber { get; set; }
        public string BankAccountType { get; set; }
        public string RoutingNumber { get; set; }
        public int? IsProcessed { get; set; }
        public string ProfileId { get; set; }
        public string PaymentProfileId { get; set; }
        public DateTime CreateDate { get; set; }
        public int InvoiceId { get; set; }
        public int MembershipId { get; set; }
        public string MembershipDescription { get; set; }
        
    }
}
