using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class WriteOffModel
    {
        public int OrganizationId { get; set; }
        public int WriteOffId { get; set; }
        public DateTime Date { get; set; }
        public int InvoiceDetailId { get; set; }
        public string Reason { get; set; }
        public decimal Amount { get; set; }
        public  InvoiceDetailModel InvoiceDetail { get; set; }
        public int UserId { get; set; }
        public StaffUserModel User { get; set; }
    }
}
