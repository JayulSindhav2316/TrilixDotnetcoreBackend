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
        Task UpdateStaffUser(Staffuser staffuserToUpdate, Staffuser staffuser);
        //Task DeleteStaffUser(Staffuser staffUser);
    }
}