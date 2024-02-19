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
    public class GroupHistoryRepository : Repository<Grouphistory>, IGrouphistoryRepository
    {
        public GroupHistoryRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<IEnumerable<Grouphistory>> GetGrouphistoryByEntityIdAsync(int entityId)
        {
            return await membermaxContext.Grouphistories
                //.Include(x => x.Group)
                //.Include(x => x.ActivityStaff)
                //.Include(x => x.GroupMember)
               //.Where(x => x.GroupMember.EntityId == entityId)
               .ToListAsync();
        }

        public async Task<IEnumerable<Grouphistory>> GetGrouphistoryByGroupIdAsync(int groupId)
        {
            return await membermaxContext.Grouphistories
                //.Include(x => x.Group)
                //.Include(x => x.ActivityStaff)
                //.Include(x => x.GroupMember)
               .Where(x => x.GroupId == groupId)
               .ToListAsync();
        }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }
    }
}
