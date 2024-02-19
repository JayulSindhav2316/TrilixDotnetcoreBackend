using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class ItemModel
    {
        public int InvoiceDetailId { get; set; }
        public int ItemId { get; set; }
        public string Name { get; set; }
        public int ItemType { get; set; }
        public string ItemCode { get; set; }
        public string Description { get; set; }
        public decimal UnitRate { get; set; }
        public int Quantity { get; set; }
        public int Status { get; set; }
        public int ItemGlAccount { get; set; }
        public string ItemTypeName { get; set; }
        public string GlAccountCode { get; set; }
        public byte[] Image { get; set; }
        public int StockCount { get; set; }
        public int EnableMemberPortal { get; set; }
        public int TotalStock { get; set; }
        public int EnableStock { get; set; }
        public int? BillableEntityId { get; set; }
        public decimal? Amount { get; set; }

    }
}
