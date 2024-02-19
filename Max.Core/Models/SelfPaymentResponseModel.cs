using System;
using System.Collections.Generic;
using System.Text;


namespace Max.Core.Models
{
    public class SelfPaymentResponseModel
    {
        public int ShoppingCartId { get; set; }
        public ShoppingCartModel ShoppingCart { get; set; }
        public InvoiceModel Invoice { get; set; }
        public ReceiptModel Receipt { get; set; }
        public List<ReceiptModel> ReceiptList { get; set; }
        public OrganizationModel Organization { get; set; }
        public int PaymentStatus { get; set; }
        public BillingAddressModel BillingAddress { get; set; }
    }
}
