using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;

namespace Max.Data.Interfaces
{
    public interface ILookupRepository : IRepository<Lookup>
    {
        Task<IEnumerable<Lookup>> GetAllLookupsAsync();
        Task<Lookup> GetLookupByIdAsync(int id);
        Task<string> GetLookupValueByGroupNameAsync(string name);

    }
}