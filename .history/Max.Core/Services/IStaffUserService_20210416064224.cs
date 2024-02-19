using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Core.Models;

namespace Max.Core.Services
{
    public interface IStaffUserService
    {
        Task<IEnumerable<StaffUser>> GetAllStaffUsers();
        Task<StaffUser> GetStaffUserById(int id);
        Task<StaffUser> CreateStaffUser(StaffUser staffUser);
        Task UpdateStaffUser(StaffUser staffUser);
        Task DeleteStaffUser(StaffUser staffUser);
    }
}