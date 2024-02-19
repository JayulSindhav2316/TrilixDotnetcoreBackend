using System;
using System.Collections.Generic;

#nullable disable

namespace Max.Data.DataModel
{
    public partial class Shoppingcart
    {
        public Shoppingcart()
        {
            Shoppingcartdetails = new HashSet<Shoppingcartdetail>();
        }

        public int ShoppingCartId { get; set; }
        public DateTime TransactionDate { get; set; }
        public int? UserId { get; set; }
        public string SessionId { get; set; }
        public int? ReceiptId { get; set; }
        public int PaymentStatus { get; set; }
        public int? EntityId { get; set; }
        public decimal CreditBalance { get; set; }
        public int UseCreditBalance { get; set; }
        public string PaymentMode { get; set; }
        public int PromoCodeId { get; set; }
        public string MemberPortalUser { get; set; }

        public virtual Receiptheader Receipt { get; set; }
        public virtual Staffuser User { get; set; }
        public virtual ICollection<Shoppingcartdetail> Shoppingcartdetails { get; set; }
    }
}
