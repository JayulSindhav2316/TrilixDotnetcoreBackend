using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Bdtransaction
    {
        public int BdtransactionId { get; set; }
        public string TokenNumber { get; set; }
        public DateTime? TransactionDate { get; set; }
        public int? PersonId { get; set; }
        public decimal? Fees { get; set; }
        public string RoutingNumber { get; set; }
        public string AccountNumber { get; set; }
        public string BankName { get; set; }
        public string PayerName { get; set; }
        public string AccountType { get; set; }
        public string ResponseCode { get; set; }
        public int? Result { get; set; }
        public int? IsAdjusted { get; set; }
        public string ReferenceNumber { get; set; }
        public int? Status { get; set; }
        public int? ActualResult { get; set; }
        public int? ReceiptId { get; set; }
        public string Partner { get; set; }
        public string ReturnCode { get; set; }
        public DateTime? EntryDate { get; set; }

        public virtual Person Person { get; set; }
        public virtual Receiptheader Receipt { get; set; }
    }
}
