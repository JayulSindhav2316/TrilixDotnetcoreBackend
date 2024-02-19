using Max.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Max.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Max.Data.Interfaces;
using Max.Data.Repositories;
using Max.Data.DataModel;
using Max.Core;
using System.Numerics;
using Max.Core.Helpers;
using System.Linq;

namespace Max.Data.Repositories
{
    public class GroupRegistrationRepository: Repository<Registrationgroup>, IGroupRegistrationRepository
    {
        public GroupRegistrationRepository(membermaxContext context)
          : base(context)
        { }

        public async Task<bool> RegisterGroup(Registrationgroup group)
        {
           await membermaxContext.Registrationgroups.AddAsync(group);
            return true;
        }

        public async Task<List<Registrationgroup>> GetAll(string searchText)
        {
            if (searchText=="Active")
            {
                var data = await membermaxContext.Registrationgroups.Include(x => x.Registrationgroupmembershiplinks)
               .ThenInclude(x => x.Membership).ThenInclude(x => x.CategoryNavigation).Where(x=>x.Status==1).
               AsNoTracking().ToListAsync();
                return data;
            }
            else
            {
                var data = await membermaxContext.Registrationgroups.Include(x => x.Registrationgroupmembershiplinks)
              .ThenInclude(x => x.Membership).ThenInclude(x => x.CategoryNavigation).
              AsNoTracking().ToListAsync();
                return data;
            }
        }
        public async Task<bool>LinkMembership(Registrationgroupmembershiplink group)
        {
            await membermaxContext.Registrationgroupmembershiplinks.AddAsync(group);
            membermaxContext.SaveChanges();
            membermaxContext.Entry(group).State = EntityState.Detached;
            return true;
        }
        public async Task<bool> DeleteLink(int linkId)
        {
            var entity = membermaxContext.Registrationgroupmembershiplinks.FirstOrDefault(x => x.RegistrationGroupMembershipLinkId == linkId);
            membermaxContext.Registrationgroupmembershiplinks.Remove(entity);

            return true;
        }

        public async Task<List<Registrationgroupmembershiplink>> GetLinkTypesByGroupId(int groupId)
        {
            var types = await membermaxContext.Registrationgroupmembershiplinks.Where(x => x.RegistrationGroupId == groupId).ToListAsync();
            return types;
        }

        public async Task<Registrationgroup> GetGroupByNameAsync(string groupName)
        {
            return await membermaxContext.Registrationgroups
            .Where(x => x.Name == groupName)
            .SingleOrDefaultAsync();
        }
        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }
    }
}
