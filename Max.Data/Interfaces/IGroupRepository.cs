using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Max.Data.DataModel;

namespace Max.Data.Interfaces
{
    public interface IGroupRepository : IRepository<Group>
    {
        Task<IEnumerable<Group>> GetAllGroupDetailsByOrganizationIdAsync(int organizationId);
        Task<IEnumerable<Group>> GetAllGroupsByOrganizationIdAsync(int organizationId);
        Task<Group> GetGroupByIdAsync(int id);
        Task<Group> GetGroupByGroupIdAsync(int groupId);
        Task<Group> GetGroupBySocialGroupIdAsync(int id);
        Task<Group> GetGroupByNameAsync(string name);
        Task<IEnumerable<Group>> GetAllGroupsByEntityIdAsync(int entityid);
        Task<IEnumerable<Group>> GetGroupsByEntityIdAsync(int entityid);
    }
}
