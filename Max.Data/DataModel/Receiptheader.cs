using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Receiptheader
    {
        public Receiptheader()
        {
            Autobillingpayments = new HashSet<Autobillingpayment>();
            Journalentryheaders = new HashSet<Journalentryheader>();
            Paymenttransactions = new HashSet<Paymenttransaction>();
            Receiptdetails = new HashSet<Receiptdetail>();
            Shoppingcarts = new HashSet<Shoppingcart>();
            Voiddetails = new HashSet<Voiddetail>();
        }

        public int Receiptid { get; set; }
        public DateTime Date { get; set; }
        public int? StaffId { get; set; }
        public string PaymentMode { get; set; }
        public string CheckNo { get; set; }
        public int Status { get; set; }
        public int? OrganizationId { get; set; }
        public string Notes { get; set; }
        public int PromoCodeId { get; set; }
        public int Portal { get; set; }
        public int? BillableEntityId { get; set; }

        public virtual Entity BillableEntity { get; set; }
        public virtual Organization Organization { get; set; }
        public virtual Staffuser Staff { get; set; }
        public virtual ICollection<Autobillingpayment> Autobillingpayments { get; set; }
        public virtual ICollection<Journalentryheader> Journalentryheaders { get; set; }
        public virtual ICollection<Paymenttransaction> Paymenttransactions { get; set; }
        public virtual ICollection<Receiptdetail> Receiptdetails { get; set; }
        public virtual ICollection<Shoppingcart> Shoppingcarts { get; set; }
        public virtual ICollection<Voiddetail> Voiddetails { get; set; }
    }
}
