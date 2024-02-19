using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;

namespace Max.Data.Interfaces
{
    public interface IShoppingCartDetailRepository : IRepository<Shoppingcartdetail>
    {
        Task<IEnumerable<Shoppingcartdetail>> GetAllShoppingCartDetailsAsync();
        Task<Shoppingcartdetail> GetShoppingCartDetailByIdAsync(int id);
        Task<IEnumerable<Shoppingcartdetail>> GetShoppingCartDetailsByCartIdAsync(int id);
    }
}
