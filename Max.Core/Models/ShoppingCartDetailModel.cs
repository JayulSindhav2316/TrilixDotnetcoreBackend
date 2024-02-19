using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class ShoppingCartDetailModel
    {
        public int ShoppingCartDetailId { get; set; }
        public int? ShoppingCartId { get; set; }
        public string Description { get; set; }
        public int? ItemType { get; set; }
        public int? ItemId { get; set; }
        public int? Selected { get; set; }
        public decimal? Price { get; set; }
        public decimal? Discount { get; set; }
        public int? Quantity { get; set; }
        public decimal? Tax { get; set; }
        public string ItemGroup { get; set; }
        public string ItemGroupDescription { get; set; }
        public int? MembershipId { get; set; }
        public decimal Amount { get; set; }
        public decimal AmountDue
        {
            get
            {
                return Amount;
            }
        }
    }
}
