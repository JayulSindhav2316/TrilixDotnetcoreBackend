using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Core.Models;

namespace Max.Core.Repositories
{
    public interface IStaffUserRepository : IRepository<StaffUser>
    {
        Task<IEnumerable<StaffUser>> GetAllStaffUsersAsync();
        Task<StaffUser> GetStaffUserByIdAsync(int id);
    }
}