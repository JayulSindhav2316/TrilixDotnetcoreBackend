using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class PromoCodeModel
    {
        public int PromoCodeId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public int DiscountType { get; set; }
        public decimal Discount { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public int Status { get; set; }
        public DateTime? StartDate { get; set; }
        public int? GlAccountId { get; set; }
        public int TransactionType { get; set; }

        public decimal DiscountApplied { get; set; }

        public  GlAccountModel GlAccount { get; set; }
    }
}
