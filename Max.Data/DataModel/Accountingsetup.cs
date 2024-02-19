using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Accountingsetup
    {
        public int AccountSetupId { get; set; }
        public int OrganizationId { get; set; }
        public int OnlineCreditGlAccount { get; set; }
        public int OffLinePaymentGlAccount { get; set; }
        public int ProcessingFeeGlAccount { get; set; }
        public int SalesReturnGlAccount { get; set; }
        public int? WriteOffGlAccount { get; set; }

        public virtual Glaccount OffLinePaymentGlAccountNavigation { get; set; }
        public virtual Glaccount OnlineCreditGlAccountNavigation { get; set; }
        public virtual Organization Organization { get; set; }
        public virtual Glaccount ProcessingFeeGlAccountNavigation { get; set; }
        public virtual Glaccount SalesReturnGlAccountNavigation { get; set; }
        public virtual Glaccount WriteOffGlAccountNavigation { get; set; }
    }
}
