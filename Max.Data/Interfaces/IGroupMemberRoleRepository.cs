using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Max.Data.DataModel;


namespace Max.Data.Interfaces
{
    public interface IGroupMemberRoleRepository : IRepository<Groupmemberrole>
    {
        Task<IEnumerable<Groupmemberrole>> GetAllGroupMemberRolesByGroupMemberIdAsync(int groupMemberId);
        Task<Groupmemberrole> GetGroupMemberRoleById(int groupMemberRoleId);
    }
}
