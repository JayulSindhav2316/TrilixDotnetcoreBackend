using Max.Data.DataModel;
using Max.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Max.Data.Repositories
{
    public class EventRegisterRepository : Repository<Eventregister>, IEventRegisterRepository
    {
        public EventRegisterRepository(membermaxContext context)
          : base(context)
        { }

        public async Task<IEnumerable<Eventregister>> GetAllEventRegistrationsByEventIdAsync(int eventId)
        {
            return await membermaxContext.Eventregisters.Where(x => x.EventId == eventId).ToListAsync();
        }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }
    }
}
