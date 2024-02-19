using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Max.Data.DataModel;

namespace Max.Data.Interfaces
{
    public interface IEntityRoleHistoryRepository : IRepository<Entityrolehistory>
    {
        Task<IEnumerable<Entityrolehistory>> GetAllEntityRoleHistoryAsync();
        Task<Entityrolehistory> GetEntityRoleHistoryByIdAsync(int id);
        Task<IEnumerable<Entityrolehistory>> GetAllEntityRoleHistoryByEntityIdAsync(int entityId);
        Task<IEnumerable<Entityrolehistory>> GetAllEntityRoleHistoryByCompanyIdAsync(int companyId);
        Task<IEnumerable<Entityrolehistory>> GetAllEntityRoleHistoryByRoleIdAsync(int roleId);
        Task<IEnumerable<Entityrolehistory>> GetEntityRoleHistoryByTypeAsync(int entityId, int companyId, string type);
        Task<IEnumerable<Entityrolehistory>> GetEntityRoleHistoryAsync(int entityId, int companyId, int roleId);
        Task<bool> DeleteHistoryByEntityIdContactRoleId(int entityId, int contactRoleId);
    }
}
