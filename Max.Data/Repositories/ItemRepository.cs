using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Max.Data.Interfaces;
using Max.Data.Repositories;
using Max.Data.DataModel;

namespace Max.Data.Repositories
{
    public class ItemRepository : Repository<Item>, IItemRepository
    {
        public ItemRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<IEnumerable<Item>> GetAllItemsAsync()
        {
            return await membermaxContext.Items
                .Include(x => x.ItemGlAccountNavigation)
                .Include(x => x.ItemTypeNavigation)
                .ToListAsync();
        }
        public async Task<IEnumerable<Item>> GetItemsByCodeAsync(string code)
        {
            return await membermaxContext.Items
                .Where(x => x.ItemCode.StartsWith(code))
                .Include(x => x.ItemGlAccountNavigation)
                .Include(x => x.ItemTypeNavigation)
                .Take(10)
                .ToListAsync();
        }
        public async Task<IEnumerable<Item>> GetItemsByNameAsync(string name)
        {
            return await membermaxContext.Items
                .Where(x => x.Name.Contains(name))
                .Include(x => x.ItemGlAccountNavigation)
                .Include(x => x.ItemTypeNavigation)
                .Take(10)
                .ToListAsync();
        }

        public async Task<bool> UpdateItem(Item item)
        {
            Item existingItem = await membermaxContext.Items.FindAsync(item.ItemId);
            if (existingItem != null)
            {
                existingItem.StockCount = item.StockCount;
                membermaxContext.Entry(existingItem).State = EntityState.Modified;
                await membermaxContext.SaveChangesAsync();
                return true;
            }

            return false;
        }
        public async Task<Item> GetItemDetailByIdAsync(int id)
        {
            return await membermaxContext.Items
              .Include(x => x.ItemGlAccountNavigation)
              .Include(x => x.ItemTypeNavigation)
              .Include(x => x.Invoicedetails).AsNoTracking()
              .SingleOrDefaultAsync(m => m.ItemId == id);
        }
        public async Task<Item> GetItemByIdAsync(int id)
        {
            return await membermaxContext.Items
              .AsNoTracking()
              .SingleOrDefaultAsync(m => m.ItemId == id);
        }
        public async Task<Item> GetItemsByTypeAsync(int id)
        {
            return await membermaxContext.Items
                .SingleOrDefaultAsync(m => m.ItemType == id);
        }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }

    }
}
