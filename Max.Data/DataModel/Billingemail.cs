using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Billingemail
    {
        public int BillingEmailId { get; set; }
        public int BillingCycleId { get; set; }
        public int InvoiceId { get; set; }
        public int CartId { get; set; }
        public string EmailAddress { get; set; }
        public DateTime? CommunicationDate { get; set; }
        public string Response { get; set; }
        public int Status { get; set; }
        public string Token { get; set; }

        public virtual Invoice Invoice { get; set; }
    }
}
