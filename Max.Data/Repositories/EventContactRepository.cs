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
    public class EventContactRepository : Repository<Eventcontact>, IEventContactRepository
    {
        public EventContactRepository(membermaxContext context)
          : base(context)
        { }

        public async Task<IEnumerable<Eventcontact>> GetEventContactsByEventIdAsync(int eventId)
        {
            return await membermaxContext.Eventcontacts
                .Include(x => x.Event)
                .Include(x => x.Staff)
                .Where(x => x.EventId == eventId)
                .ToListAsync();
        }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }
    }
}
