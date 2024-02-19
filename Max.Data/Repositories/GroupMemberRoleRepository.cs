using System;
using System.Collections.Generic;
using System.Text;
using Max.Data.Interfaces;
using Max.Data.DataModel;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Max.Data.Repositories
{
    public class GroupMemberRoleRepository : Repository<Groupmemberrole>, IGroupMemberRoleRepository
    {
        public GroupMemberRoleRepository(membermaxContext context)
           : base(context)
        { }
        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }

        public async Task<IEnumerable<Groupmemberrole>> GetAllGroupMemberRolesByGroupMemberIdAsync(int groupMemberId)
        {
            return await membermaxContext.Groupmemberroles
                .Include(x => x.GroupRole)
                .Where(x => x.GroupMemberId == groupMemberId).ToListAsync();
        }

        public async Task<Groupmemberrole> GetGroupMemberRoleById(int groupMemberRoleId)
        {
            return await membermaxContext.Groupmemberroles
                .Include(x => x.GroupRole)
                .Include(x => x.GroupMember)
                .Where(x => x.GroupMemberRoleId == groupMemberRoleId).SingleOrDefaultAsync();
        }
    }
}
