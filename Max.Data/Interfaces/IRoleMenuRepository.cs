using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;

namespace Max.Data.Interfaces
{
  public interface IRoleMenuRepository : IRepository<Rolemenu>
  {
    Task<IEnumerable<Rolemenu>> GetMenuByRoleIdAsync(int id);
    Task<IEnumerable<Rolemenu>> GetMenuByMenuNameRoleIdAsync(int id, string menuName);
}
}