using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Core.Models;

namespace Max.Core.Services
{
    public interface IStaffUserService
    {
        Task<IEnumerable<Staffuser>> GetAllStaffUsers();
        Task<Staffuser> GetStaffUserById(int id);
        Task<Staffuser> CreateStaffUser(Staffuser staffUser);
        Task UpdateStaffUser(Staffuser staffuserToUpdate, Staffuser staffuser);
        //Task DeleteStaffUser(Staffuser staffUser);
    }
}