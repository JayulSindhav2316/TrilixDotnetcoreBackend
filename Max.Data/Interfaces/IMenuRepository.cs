using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;

namespace Max.Data.Interfaces
{
    public interface IMenuRepository : IRepository<Menu>
    {
        Task<IEnumerable<Menu>> GetAllMenusAsync();
        Task<IEnumerable<Menu>> GetMenusByParentIdAsync(int menuId);
    }
}