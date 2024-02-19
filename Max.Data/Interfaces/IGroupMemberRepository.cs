using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Max.Data.DataModel;

namespace Max.Data.Interfaces
{
    public interface IGroupMemberRepository : IRepository<Groupmember>
    {
        Task<IEnumerable<Groupmember>> GetAllGroupMembersByGroupIdAsync(int groupId);
        Task<IEnumerable<Groupmember>> GetAllGroupsByEntityIdAsync(int entityId);
        Task<Groupmember> GetGroupMemberByIdAsync(int groupMemberId);
        Task<IEnumerable<Groupmember>> GetGroupsByEntityIdAsync(int entityId);
        Task<IEnumerable<Groupmember>> GetOnlyGroupMembersByGroupIdAsync(int groupId);
    }
}
