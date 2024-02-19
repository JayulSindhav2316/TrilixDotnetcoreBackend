
using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;

namespace Max.Data.Interfaces
{
    public interface IGlAccountRepository : IRepository<Glaccount>
    {
        Task<IEnumerable<Glaccount>> GetAllGlaccountsAsync();
        Task<Glaccount> GetGlaccountByIdAsync(int id);
        Task<Glaccount> GetGlaccountByNameAsync(string name);
    }
}
