
using Max.Data.Audit;
using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    [Auditable]
    public partial class Invoice
    {
        public Invoice()
        {
            Autobillingdrafts = new HashSet<Autobillingdraft>();
            Billingemails = new HashSet<Billingemail>();
            Invoicedetails = new HashSet<Invoicedetail>();
            Paperinvoices = new HashSet<Paperinvoice>();
        }

        public int InvoiceId { get; set; }
        public DateTime Date { get; set; }
        public int? EntityId { get; set; }
        public DateTime DueDate { get; set; }
        public string BillingType { get; set; }
        public string InvoiceType { get; set; }
        public int? MembershipId { get; set; }
        public int? BillableEntityId { get; set; }
        public int Status { get; set; }
        public int? PaymentTransactionId { get; set; }
        public string Notes { get; set; }
        public int? UserId { get; set; }
        public int PromoCodeId { get; set; }
        public int? InvoiceItemType { get; set; }
        public int? EventId { get; set; }

        public virtual Entity BillableEntity { get; set; }
        public virtual Entity Entity { get; set; }
        public virtual Event Event { get; set; }
        public virtual Membership Membership { get; set; }
        public virtual Paymenttransaction PaymentTransaction { get; set; }
        public virtual ICollection<Autobillingdraft> Autobillingdrafts { get; set; }
        public virtual ICollection<Billingemail> Billingemails { get; set; }
        public virtual ICollection<Invoicedetail> Invoicedetails { get; set; }
        public virtual ICollection<Paperinvoice> Paperinvoices { get; set; }
    }
}
