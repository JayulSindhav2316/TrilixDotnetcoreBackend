using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Journalentrydetail
    {
        public int JournalEntryDetailId { get; set; }
        public int JournalEntryHeaderId { get; set; }
        public int? ReceiptDetailId { get; set; }
        public string GlAccountCode { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string EntryType { get; set; }

        public virtual Journalentryheader JournalEntryHeader { get; set; }
        public virtual Receiptdetail ReceiptDetail { get; set; }
    }
}
