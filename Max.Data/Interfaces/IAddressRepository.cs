using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;

namespace Max.Data.Interfaces
{
    public interface IAddressRepository : IRepository<Address>
    {
        Task<IEnumerable<Address>> GetAllAddressesAsync();
        Task<Address> GetAddressByIdAsync(int id);

        Task<IEnumerable<Address>> GetAddressByPersonIdAsync(int id);
        Task<IEnumerable<Address>> GetAddressByCompanyIdAsync(int id);
    }
}
