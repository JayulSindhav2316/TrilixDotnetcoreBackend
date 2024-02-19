using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;

namespace Max.Data.Interfaces
{
    public interface IShoppingCartRepository : IRepository<Shoppingcart>
    {
        Task<IEnumerable<Shoppingcart>> GetAllShoppingCartsAsync();
        Task<Shoppingcart> GetShoppingCartByIdAsync(int id);
        Task<Shoppingcart> GetShoppingCartByUserIdAsync(int id);
        Task<IEnumerable<Shoppingcart>> GetShoppingCartItemsByUserIdAsync(int id);
        Task<Shoppingcart> GetMemberPortalShoppingCartByEntityIdAsync(int entityId, string memberPortalUser);
        Task<Shoppingcart> GetMemberPortalShoppingCartByUserNameAsync(string memberPortalUser);
    }
}
