
using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;

namespace Max.Data.Interfaces
{
    public interface IGlAccountTypeRepository : IRepository<Glaccounttype>
    {
        Task<IEnumerable<Glaccounttype>> GetAllGlAccountTypesAsync();
        Task<Glaccounttype> GetGlAccountTypeByIdAsync(int id);

    }
   
}
