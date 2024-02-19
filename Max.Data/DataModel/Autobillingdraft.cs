using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Autobillingdraft
    {
        public Autobillingdraft()
        {
            Autobillingpayments = new HashSet<Autobillingpayment>();
        }

        public int AutoBillingDraftId { get; set; }
        public int? BillingDocumentId { get; set; }
        public string Name { get; set; }
        public int EntityId { get; set; }
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
        public int? MembershipId { get; set; }
        public int InvoiceId { get; set; }

        public virtual Billingdocument BillingDocument { get; set; }
        public virtual Entity Entity { get; set; }
        public virtual Invoice Invoice { get; set; }
        public virtual Membership Membership { get; set; }
        public virtual ICollection<Autobillingpayment> Autobillingpayments { get; set; }
    }
}
