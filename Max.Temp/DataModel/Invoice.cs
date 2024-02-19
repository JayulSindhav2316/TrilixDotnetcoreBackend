using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Temp.DataModel
{
    public partial class Invoice
    {
        public Invoice()
        {
            Invoicedetails = new HashSet<Invoicedetail>();
        }

        public int InvoiceId { get; set; }
        public DateTime? Date { get; set; }
        public int? PersonId { get; set; }
        public DateTime? DueDate { get; set; }
        public string BillingType { get; set; }
        public string InvoiceType { get; set; }
        public int? MembershipId { get; set; }
        public int? BillablePersonId { get; set; }
        public int? CreditCardTransactionId { get; set; }
        public int? BankDraftTransactionId { get; set; }
        public int? Status { get; set; }

        public virtual Person BillablePerson { get; set; }
        public virtual Membership Membership { get; set; }
        public virtual ICollection<Invoicedetail> Invoicedetails { get; set; }
    }
}
