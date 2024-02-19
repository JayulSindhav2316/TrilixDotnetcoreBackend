using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Max.Data.Interfaces;
using Max.Data.Repositories;
using Max.Data.DataModel;

namespace Max.Data.Repositories
{
  public class MenuRepository : Repository<Menu>, IMenuRepository
  {
    public MenuRepository(membermaxContext context)
        : base(context)
    { }

    public async Task<IEnumerable<Menu>> GetAllMenusAsync()
    {
      return await membermaxContext.Menus
          .OrderBy(x => x.DisplayOrder)
          .ToListAsync();
    }

    public async Task<IEnumerable<Menu>> GetMenusByParentIdAsync(int menuId)
    {
        return await membermaxContext.Menus.Where(x => x.ParentMenuId == menuId)
                .OrderBy(x => x.DisplayOrder)
                .ToListAsync();
    }

    private membermaxContext membermaxContext
    {
      get { return Context as membermaxContext; }
    }
  }
}
