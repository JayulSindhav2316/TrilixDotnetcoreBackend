using System;
using System.Collections.Generic;
using System.Text;
using Max.Data.Interfaces;
using Max.Data.DataModel;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Max.Core;

namespace Max.Data.Repositories
{
    public class GroupRepository : Repository<Group>, IGroupRepository
    {
        public GroupRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<IEnumerable<Group>> GetAllGroupsByOrganizationIdAsync(int organizationId)
        {
            return await membermaxContext.Groups
                .Where(x => x.OrganizationId == organizationId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Group>> GetAllGroupDetailsByOrganizationIdAsync(int organizationId)
        {
            return await membermaxContext.Groups
            .Include(x => x.Groupmembers.Where(s => s.Status == 1))
            .Where(x => x.OrganizationId == organizationId)
            .ToListAsync();
        }

        public async Task<Group> GetGroupByIdAsync(int id)
        {
            return await membermaxContext.Groups
                .Include(x => x.Containeraccesses)
                .Include(x => x.Documentaccesses)
                .Include(x => x.Groupmembers)
                .SingleOrDefaultAsync(x => x.GroupId == id);
        }

        public async Task<Group> GetGroupByGroupIdAsync(int groupId)
        {
            return await membermaxContext.Groups
                .SingleOrDefaultAsync(x => x.GroupId == groupId);
        }

        public async Task<Group> GetGroupByNameAsync(string name)
        {
            return await membermaxContext.Groups
                .SingleOrDefaultAsync(x => x.GroupName == name);
        }

        public async Task<IEnumerable<Group>> GetAllGroupsByEntityIdAsync(int entityid)
        {
            return await membermaxContext.Groups
                .Include(x => x.Groupmembers.Where(x => x.EntityId == entityid))
                    .ThenInclude(x => x.Groupmemberroles)
                        .ThenInclude(x => x.GroupRole)
                .Where(x => x.Groupmembers.Any(x => x.EntityId == entityid && x.Status == (int)Status.Active && x.Group.Status == (int)Status.Active))
                .ToListAsync();
        }

        public async Task<IEnumerable<Group>> GetGroupsByEntityIdAsync(int entityid)
        {
            return await membermaxContext.Groups
                .Include(x => x.Groupmembers.Where(x => x.EntityId == entityid))
                    .ThenInclude(x => x.Groupmemberroles)
                        .ThenInclude(x => x.GroupRole)
                .Where(x => x.Groupmembers.Any(x => x.EntityId == entityid))
                .ToListAsync();
        }

        public async Task<Group> GetGroupBySocialGroupIdAsync(int id)
        {
            return await membermaxContext.Groups
                .Include(x => x.Containeraccesses)
                .Include(x => x.Documentaccesses)
                .Include(x => x.Groupmembers)
                .SingleOrDefaultAsync(x => x.SocialGroupId == id);
        }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }
    }
}
