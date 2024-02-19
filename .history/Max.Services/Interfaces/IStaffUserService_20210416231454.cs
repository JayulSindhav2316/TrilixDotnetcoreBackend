using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;

namespace Max.Services.Interfaces
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