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
    public class LinkEventFeeTypeRepository : Repository<Linkeventfeetype>, ILinkEventFeeTypeRepository
    {
        public LinkEventFeeTypeRepository(membermaxContext context)
           : base(context)
        { }

        public async Task<IEnumerable<Linkeventfeetype>> GetFeeTypesByEventIdAsync(int eventId)
        {
            return await membermaxContext.Linkeventfeetypes
               .Include(x => x.RegistrationFeeType)
               .Where(x => x.EventId == eventId)
               .ToListAsync();
        }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }
    }
}
