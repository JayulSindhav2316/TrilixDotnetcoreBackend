using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class ReceiptLineItem
    {

        public string ItemGroupDescription { get; set; }
        public string MembershipCategory { get; set; }
        public string MembershipName { get; set; }
        public string MembershipPeriod { get; set; }
        public string EventName { get; set; }
        public string EventType { get; set; }
        public string Description { get; set; }
        public string Quantity { get; set; }
        public string Rate { get; set; }
        public string Tax { get; set; }
        public string Discount { get; set; }
        public string Amount { get; set; }
    }
}
