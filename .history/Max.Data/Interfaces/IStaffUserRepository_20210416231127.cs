using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;

namespace Max.Data.Interfaces
{
    public interface IStaffUserRepository : IRepository<Staffuser>
    {
        Task<IEnumerable<Staffuser>> GetAllStaffUsersAsync();
        Task<Staffuser> GetStaffUserByIdAsync(int id);
    }
}