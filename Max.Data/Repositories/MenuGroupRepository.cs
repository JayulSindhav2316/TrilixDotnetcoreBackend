using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Max.Data.Interfaces;
using Max.Data.Repositories;
using Max.Data.DataModel;

namespace Max.Data.Repositories
{
    public class MenuGroupRepository : Repository<Menugroup>, IMenuGroupRepository
    {
        public MenuGroupRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<IEnumerable<Menugroup>> GetAllMenuGroupsAsync()
        {
            return await membermaxContext.Menugroups
                .Include(x => x.Menus)
                .OrderBy(x => x.DisplayOrder)
                .ThenBy(x => x.Menus).OrderBy(x => x.DisplayOrder)
                .ToListAsync();
        }

        public IEnumerable<string> GetAllMenusAsync()
        {
            return membermaxContext.Menugroups
                .OrderBy(x => x.DisplayOrder)
                .GroupBy(x => x.MenuName)
                .Select(x => x.Key)
                .ToList();
        }
        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }
    }
}
