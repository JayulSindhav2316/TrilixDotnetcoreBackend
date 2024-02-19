using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Voiddetail
    {
        public int VoidId { get; set; }
        public DateTime VoidDate { get; set; }
        public string VoidMode { get; set; }
        public int ReceiptId { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string Reason { get; set; }
        public int UserId { get; set; }
        public int PaymentTransactionId { get; set; }
        public int EntityId { get; set; }

        public virtual Receiptheader Receipt { get; set; }
        public virtual Staffuser User { get; set; }
    }
}
