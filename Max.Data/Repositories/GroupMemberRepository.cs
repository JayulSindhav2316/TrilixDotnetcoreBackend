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
    public class GroupMemberRepository : Repository<Groupmember>, IGroupMemberRepository
    {
        public GroupMemberRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<IEnumerable<Groupmember>> GetAllGroupMembersByGroupIdAsync(int groupId)
        {
            return await membermaxContext.Groupmembers
                 .Include(x => x.Entity)
                    .ThenInclude(x => x.People)
                        .ThenInclude(x => x.Emails)
                .Include(x => x.Entity)
                    .ThenInclude(x => x.People)
                        .ThenInclude(x => x.Phones)
                .Include(x => x.Group)
                .Include(x => x.Groupmemberroles)
                    .ThenInclude(x => x.GroupRole)
                .Where(x => x.GroupId == groupId).ToListAsync();
        }
        public async Task<IEnumerable<Groupmember>> GetOnlyGroupMembersByGroupIdAsync(int groupId)
        {
            return await membermaxContext.Groupmembers
                .Include(x => x.Group)
                .Where(x => x.GroupId == groupId).ToListAsync();
        }


        public async Task<IEnumerable<Groupmember>> GetAllGroupsByEntityIdAsync(int entityId)
        {
            return await membermaxContext.Groupmembers
                //.Include(x => x.Person)
                .Include(x => x.Group)
                //.Include(x => x.Role)
                .Where(x => x.EntityId == entityId).ToListAsync();
        }

        public async Task<IEnumerable<Groupmember>> GetGroupsByEntityIdAsync(int entityId)
        {
            return await membermaxContext.Groupmembers
                .Include(x => x.Groupmemberroles)
                    .ThenInclude(x => x.GroupRole)
                .Include(x => x.Group)
                .Where(x => x.EntityId == entityId).ToListAsync();
        }

        public async Task<Groupmember> GetGroupMemberByIdAsync(int groupMemberId)
        {
            return await membermaxContext.Groupmembers
                .Include(x => x.Groupmemberroles)
                .Where(x => x.GroupMemberId == groupMemberId).SingleOrDefaultAsync();
        }
        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }
    }
}
