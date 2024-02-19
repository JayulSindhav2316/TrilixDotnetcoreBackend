using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;

namespace Max.Data.Interfaces
{
    public interface IPhoneRepository : IRepository<Phone>
    {
        Task<IEnumerable<Phone>> GetAllPhonesAsync();
        Task<Phone> GetPhoneByIdAsync(int id);
    }
}
