using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Core.Models
{
    public class ShoppingCartModel
    {
        public ShoppingCartModel()
        {
            ShoppingCartDetails = new List<ShoppingCartDetailModel>();
        }

        public int ShoppingCartId { get; set; }
        public DateTime? TransactionDate { get; set; }
        public int? UserId { get; set; }
        public string SessionId { get; set; }
        public string MemberPortalUser { get; set; }
        public int? ReceiptId { get; set; }
        public int? PaymentStatus { get; set; }
        public decimal CreditBalance { get; set; }
        public int UseCreditBalance { get; set; }
        public int PersonId { get; set; }
        public int PromoCodeId { get; set; }
        public string PromoCode { get; set; }
        public string BillableEntityName { get; set; }
        public int EntityId { get; set; }
        public virtual ReceiptHeaderModel Receipt { get; set; }
        public virtual StaffUserModel User { get; set; }
        public List<ShoppingCartDetailModel> ShoppingCartDetails { get; set; }


    }
}
