using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;
using Max.Core.Models;
using Max.Core;

namespace Max.Services.Interfaces
{
    public interface IShoppingCartService
    {
        Task<IEnumerable<ShoppingCartModel>> GetAllShoppingCarts();
        Task<ShoppingCartModel> GetShoppingCartById(int id);
        Task<ShoppingCartModel> GetShoppingCartByUserId(int id);
        Task<ShoppingCartModel> GetMemberPortalShoppingCartByEntityId(int entityId, string userName);
        Task<ShoppingCartModel> CreateShoppingCart(ShoppingCartModel model);
        Task<ShoppingCartModel> AddItemToShoppingCart(ShoppingCartDetailModel model);
        Task<bool> UpdateShoppingCartPaymentStatus(int userId, int cartId, int status, decimal creditUsed, string paymentMode);
        Task<ShoppingCartModel> AddInvoiceToShoppingCart(int userid, int invoiceId);
        Task<ShoppingCartModel> AddReceiptToShoppingCart(int userid, int useCreditBalance);
        Task<bool> DeleteShoppingCart(int shoppingCartId);
        Task<bool> ProcessCheckPayment(CheckPaymentModel model);
        Task<bool> ProcessOffLinePayment(OffLinePaymentModel model);
        Task<ShoppingCartModel> ApplyPromoCode(int id, string promoCode, decimal superDiscount);
        Task<ShoppingCartModel> DeletePromoCode(int id, string promoCode);
        Task<ShoppingCartModel> AddMemberPortalInvoiceToShoppingCart(int invoiceId);
        Task<SelfPaymentResponseModel> AddMemberPortalReceiptToShoppingCart(int cartId, int useCreditBalance);
        Task<ShoppingCartModel> AddInvoicesToMemberPortalShoppingCart(string userName,string invoiceIds);
        Task<ShoppingCartDetailModel> UpdateShoppingCartItem(ShoppingCartDetailModel model);
        Task<ShoppingCartModel> DeleteShoppingCartItem(ShoppingCartDetailModel model);
        Task<bool> UpdateInvoiceDetails(int id, string code, decimal superDiscount);
        List<string> GetOfflinePaymentTypes();
    }
}
