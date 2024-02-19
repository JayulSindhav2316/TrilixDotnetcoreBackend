using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class RefundResponseModel
    {
        public int UserId { get; set; }
        public int ReceiptId { get; set; }
        public int InvoiceDetailId { get; set; }
        public int ReceiptDetailId { get; set; }
        public int RefundDetailId { get; set; }
        public int Status { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        
    }
}
