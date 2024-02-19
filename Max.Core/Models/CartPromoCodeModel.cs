using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class CartPromoCodeModel
    {
        public int CartId { get; set; }
        public string PromoCode { get; set; }
        public decimal Discount { get; set; }

    }
}
