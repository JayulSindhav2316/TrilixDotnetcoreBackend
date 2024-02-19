using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Core.Models;

namespace Max.Core.Repositories
{
    public interface IStaffUserRepository : IRepository<Staffuser>
    {
        Task<IEnumerable<Staffuser>> GetAllStaffUsersAsync();
        Task<Staffuser> GetStaffUserByIdAsync(int id);
    }
}