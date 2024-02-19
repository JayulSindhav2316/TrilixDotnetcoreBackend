using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Item
    {
        public Item()
        {
            Invoicedetails = new HashSet<Invoicedetail>();
        }

        public int ItemId { get; set; }
        public string Name { get; set; }
        public int ItemType { get; set; }
        public string ItemCode { get; set; }
        public string Description { get; set; }
        public decimal UnitRate { get; set; }
        public int Status { get; set; }
        public int ItemGlAccount { get; set; }
        public byte[] Image { get; set; }
        public int? StockCount { get; set; }
        public int? EnableMemberPortal { get; set; }
        public int? TotalStock { get; set; }
        public int? EnableStock { get; set; }

        public virtual Glaccount ItemGlAccountNavigation { get; set; }
        public virtual Itemtype ItemTypeNavigation { get; set; }
        public virtual ICollection<Invoicedetail> Invoicedetails { get; set; }
    }
}
