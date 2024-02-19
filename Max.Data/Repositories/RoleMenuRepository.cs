using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Max.Data.Interfaces;
using Max.Data.Repositories;
using Max.Data.DataModel;

namespace Max.Data.Repositories
{
  public class RoleMenuRepository : Repository<Rolemenu>, IRoleMenuRepository
  {
    public RoleMenuRepository(membermaxContext context)
        : base(context)
    { }

    public async Task<IEnumerable<Rolemenu>> GetMenuByRoleIdAsync(int id)
    {
      return await membermaxContext.Rolemenus.Where(x => x.RoleId == id)
                .Include(x => x.Menu)
                .OrderBy(x => x.Menu.Group.DisplayOrder)
                .ThenBy(x => x.Menu.GroupId)
                .ThenBy(x => x.Menu.DisplayOrder)
                .ToListAsync();
    }

    public async Task<IEnumerable<Rolemenu>> GetMenuByMenuNameRoleIdAsync(int id, string menuName)
    {
        return await membermaxContext.Rolemenus.Where(x => x.RoleId == id)
                    .Include(x => x.Menu)
                    .ThenInclude(x => x.Group)
                    .OrderBy(x => x.Menu.DisplayOrder)
                    .ThenBy(x => x.Menu.GroupId)
                    .Where(x => x.Menu.Group.MenuName == menuName)
                    .ToListAsync();
    }
    private membermaxContext membermaxContext
    {
      get { return Context as membermaxContext; }
    }
  }
}