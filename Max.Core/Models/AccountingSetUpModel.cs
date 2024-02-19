using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class AccountingSetUpModel
    {
        public int OrganizationId { get; set; }
        public int OnlineCreditGlAccount { get; set; }
        public int OffLinePaymentGlAccount { get; set; }
        public int ProcessingFeeGlAccount { get; set; }
        public int SalesReturnGlAccount { get; set; }
        public int WriteOffGlAccount { get; set; }
    }
}
