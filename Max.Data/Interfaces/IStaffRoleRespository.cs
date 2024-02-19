using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;

namespace Max.Data.Interfaces
{
    public interface IStaffRoleRepository : IRepository<Staffrole>
    {
        Task<IEnumerable<Staffrole>> GetAllStaffRolesAsync();
        Task<Staffrole> GetStaffRoleByIdAsync(int id);
        Task<IEnumerable<Staffrole>> GetAllStaffRolesByStaffIdAsync(int staffId);
        Task<IEnumerable<Staffrole>> GetAllStaffRolesByRoleIdAsync(int roleId);

    }
}
