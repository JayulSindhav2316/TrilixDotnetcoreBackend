using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Max.Data.DataModel;
using Max.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Max.Data.Repositories
{
    public class LinkEventGroupRepository : Repository<Linkeventgroup>, ILinkEventGroupRepository
    {
        public LinkEventGroupRepository(membermaxContext context)
          : base(context)
        { }

        public async Task<Linkeventgroup> GetLinkEventGroupByRegistrationGroupIdAndEventIdAsync(int registrationGroupId, int eventId)
        {
            return await membermaxContext.Linkeventgroups
               .Include(x => x.Linkregistrationgroupfees)
               .Include(x => x.RegistrationGroup)
               .Include(x => x.Event)
               .Where(x => x.RegistrationGroupId == registrationGroupId && x.EventId == eventId)
               .SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<Linkeventgroup>> GetLinkEventGroupByEventIdAsync(int eventId)
        {
            return await membermaxContext.Linkeventgroups
               .Include(x => x.Linkregistrationgroupfees)
               .Include(x => x.RegistrationGroup)
               .Include(x => x.Event)
               .Where(x => x.EventId == eventId)
               .ToListAsync();
        }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }

    }
}
