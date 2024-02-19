using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;
using Max.Core.Models;

namespace Max.Services.Interfaces
{
    public interface IStaffRoleService
    {
        Task<IEnumerable<Staffrole>> GetAllStaffRoles();
        Task<Staffrole> GetStaffRoleById(int id);
        Task<IEnumerable<Staffrole>> GetStaffRoleByStaffId(int id);
        Task<Staffrole> CreateStaffRole(int staffId, int  roleId);
        Task DeleteStaffRole(int staffRoleId);
    }
}