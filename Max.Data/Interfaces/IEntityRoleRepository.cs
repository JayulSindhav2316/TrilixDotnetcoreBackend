using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Max.Data.DataModel;

namespace Max.Data.Interfaces
{
    public interface IEntityRoleRepository : IRepository<Entityrole>
    {
        Task<IEnumerable<Entityrole>> GetAllEntityRolesAsync();
        Task<Entityrole> GeActiveEntityRoleByIdAsync(int id);
        Task<Entityrole> GetEntityRoleByIdAsync(int id);
        Task<IEnumerable<Entityrole>> GetAllEntityRolesByEntityIdAsync(int entityId);
        Task<IEnumerable<Entityrole>> GetActiveEntityRolesByEntityIdAsync(int entityId);
        Task<List<Entityrole>> GetActiveEntityRolesByCompanyIdAsync(int companyId);
        Task<List<Entityrole>> GetContactsByFirstAndLastNameAsync(string firstName, string lastName, int companyId);
        Task<List<Entityrole>> GetEntityByRoleAndCompanyIdAsync(int roleId, int entityId);
        Task<IEnumerable<Entityrole>> GetEntityRolesByEntityIdAsync(int entityId);
        Task<IEnumerable<Entityrole>> GetAllEntityRolesByCompanyIdAsync(int companyId);
        Task<IEnumerable<Entityrole>> GetAllEntityRolesByEntityIdContactRoleIdAndCompanyIdAsync(int entityId,int contactRoleId,int companyId);
    }
}
