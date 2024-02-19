using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Max.Data.DataModel;

namespace Max.Data.Interfaces
{
    public interface IMenuGroupRepository : IRepository<Menugroup>
    {
        Task<IEnumerable<Menugroup>> GetAllMenuGroupsAsync();
        IEnumerable<string> GetAllMenusAsync();
    }
}