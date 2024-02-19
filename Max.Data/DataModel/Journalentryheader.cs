using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Journalentryheader
    {
        public Journalentryheader()
        {
            Journalentrydetails = new HashSet<Journalentrydetail>();
        }

        public int JournalEntryHeaderId { get; set; }
        public DateTime EntryDate { get; set; }
        public int? ReceiptHeaderId { get; set; }
        public int UserId { get; set; }
        public string TransactionType { get; set; }

        public virtual Receiptheader ReceiptHeader { get; set; }
        public virtual Staffuser User { get; set; }
        public virtual ICollection<Journalentrydetail> Journalentrydetails { get; set; }
    }
}
