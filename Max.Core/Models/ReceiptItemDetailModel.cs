using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class ReceiptItemDetailModel
    {
        public int ReciptItemDetailId { get; set; }
        public int? ReceiptDetailId { get; set; }
        public int? ItemType { get; set; }
        public decimal? CurrentPrice { get; set; }
        public decimal? Amount { get; set; }
        public decimal? RunningPrice { get; set; }
        public int? Status { get; set; }
        public int? GlaccountId { get; set; }
        public decimal? AmountEdited { get; set; }
    }
}
