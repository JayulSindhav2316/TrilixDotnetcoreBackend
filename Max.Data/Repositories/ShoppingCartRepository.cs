using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Max.Data.Interfaces;
using Max.Data.Repositories;
using Max.Data.DataModel;
using Max.Core;
using System;

namespace Max.Data.Repositories
{
    public class ShoppingCartRepository : Repository<Shoppingcart>, IShoppingCartRepository
    {
        public ShoppingCartRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<IEnumerable<Shoppingcart>> GetAllShoppingCartsAsync()
        {
            return await membermaxContext.Shoppingcarts
                .Include(x => x.Shoppingcartdetails)
                .ToListAsync();
        }

        public async Task<Shoppingcart> GetShoppingCartByIdAsync(int id)
        {
            return await membermaxContext.Shoppingcarts
                .Include(x => x.Shoppingcartdetails)
                .SingleOrDefaultAsync(m => m.ShoppingCartId == id);
        }

        /// <summary>
        /// Returns Shopping cart which are still in process
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Shoppingcart> GetShoppingCartByUserIdAsync(int id)
        {
            return await membermaxContext.Shoppingcarts
                .Include(x => x.Shoppingcartdetails)
                .Where(m => m.UserId == id && m.PaymentStatus != (int)PaymentTransactionStatus.Approved  && m.TransactionDate.Date == DateTime.Now.Date)
                .OrderByDescending(x => x.TransactionDate)
                .SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<Shoppingcart>> GetShoppingCartItemsByUserIdAsync(int id)
        {
            return await membermaxContext.Shoppingcarts
                .Include(x => x.Shoppingcartdetails)
                .Where(m => m.UserId == id && m.PaymentStatus != (int)PaymentTransactionStatus.Approved && m.TransactionDate.Date == DateTime.Now.Date)
                .OrderByDescending(x => x.TransactionDate)
                .ToListAsync();
        }

        public async Task<Shoppingcart> GetMemberPortalShoppingCartByEntityIdAsync(int entityId, string memberPortalUser)
        {
            return await membermaxContext.Shoppingcarts
                .Include(x => x.Shoppingcartdetails)
                .Where(m => m.MemberPortalUser == memberPortalUser && m.EntityId == entityId && m.PaymentStatus != (int)PaymentTransactionStatus.Approved && m.TransactionDate.Date == DateTime.Now.Date)
                .OrderByDescending(x => x.TransactionDate)
                .FirstOrDefaultAsync();
        }
        
        public async Task<Shoppingcart> GetMemberPortalShoppingCartByUserNameAsync(string memberPortalUser)
        {
            return await membermaxContext.Shoppingcarts
                .Include(x => x.Shoppingcartdetails)
                .Where(m => m.MemberPortalUser == memberPortalUser && m.PaymentStatus != (int)PaymentTransactionStatus.Approved && m.TransactionDate.Date == DateTime.Now.Date)
                .OrderByDescending(x => x.TransactionDate)
                .FirstOrDefaultAsync();
        }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }

    }
}
