using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;
using Max.Core.Models;

namespace Max.Services.Interfaces
{
    public interface IStaffUserService
    {
        Task<IEnumerable<Staffuser>> GetAllStaffUsers();
        Task<Staffuser> GetStaffUserById(int id);
        Task<Staffuser> CreateStaffUser(StaffUserModel  staffUser);
        Task<bool> UpdateStaffUser(StaffUserModel staffUser);
        Task DeleteStaffUser(Staffuser staffUser);
        Task<bool> AssignRole(int staffUserId, int roleId);
        Task<bool> Authenticate(string userName, string password);
    }
}