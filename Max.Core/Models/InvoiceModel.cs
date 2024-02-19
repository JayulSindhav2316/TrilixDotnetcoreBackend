
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Max.Core.Models
{
    public class InvoiceModel
    {
        public InvoiceModel()
        {
            InvoiceDetails = new List<InvoiceDetailModel>();
        }

        public int InvoiceId { get; set; }
        public DateTime Date { get; set; }
        public int EntityId { get; set; }
        public DateTime DueDate { get; set; }
        public string BillingType { get; set; }
        public string InvoiceType { get; set; }
        public int? MembershipId { get; set; }
        public int? BillableEntityId { get; set; }
        public int? PaymentTransactionId { get; set; }
        public int Status { get; set; }
        public string Notes { get; set; }
        public int UserId { get; set; }
        public int Quantity { get; set; }
        public string UserName { get; set; }
        public string BillingEmail { get; set; }
        public EntityModel BillableEntity { get; set; }
        public EntityModel Entity { get; set; }
        public BillingAddressModel BillingAddress { get; set; }
        public  MembershipModel Membership { get; set; }
        public OrganizationModel Organization { get; set; }
        public PaymentTransactionModel PaymentTransaction { get; set; }
        public EventModel Event { get; set; }
        public decimal Discount { get; set; }
        public int PromoCodeId { get; set; }
        public decimal AvailableCredit { get; set; }
        public bool IsAllInvoiceDetailsWriteOff { get; set; }
        public decimal Amount
        {
            get
            {
                return InvoiceDetails.Sum(x => (x.Amount));
            }
        }
        public decimal BalanceAmount { get; set; }
        public bool IsBalanceAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public int? EventId { get; set; }
        public List<InvoiceDetailModel> InvoiceDetails { get; set; }
    }
}

