using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Paymentprocessor
    {
        public int PaymentProcessorId { get; set; }
        public string Name { get; set; }
        public string MerchantId { get; set; }
        public string LiveUrl { get; set; }
        public string LiveAcceptJsurl { get; set; }
        public string TestUrl { get; set; }
        public string TestAcceptJsurl { get; set; }
        public string LoginId { get; set; }
        public string ApiKey { get; set; }
        public string TransactionKey { get; set; }
        public int? TransactionMode { get; set; }
        public int? OrganizationId { get; set; }

        public virtual Organization Organization { get; set; }
    }
}
