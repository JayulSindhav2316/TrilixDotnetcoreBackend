using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Max.Data.Interfaces;
using Max.Data.Repositories;
using Max.Data.DataModel;

namespace Max.Data.Repositories
{
    public class ShoppingCartDetailRepository : Repository<Shoppingcartdetail>, IShoppingCartDetailRepository
    {
        public ShoppingCartDetailRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<IEnumerable<Shoppingcartdetail>> GetAllShoppingCartDetailsAsync()
        {
            return await membermaxContext.Shoppingcartdetails
                .Include(x => x.ShoppingCart)
                .ToListAsync();
        }

        public async Task<Shoppingcartdetail> GetShoppingCartDetailByIdAsync(int id)
        {
            return await membermaxContext.Shoppingcartdetails
                .Include(x => x.ShoppingCart)
                .SingleOrDefaultAsync(m => m.ShoppingCartDetailId == id);
        }

        public async Task<IEnumerable<Shoppingcartdetail>> GetShoppingCartDetailsByCartIdAsync(int id)
        {
            return await membermaxContext.Shoppingcartdetails
                .Where( x => x.ShoppingCartId==id)
                .Include(x => x.ShoppingCart)
                .ToListAsync();
        }


        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }

    }
}
