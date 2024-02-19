using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Max.Data.DataModel;
using Max.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Max.Data.Repositories
{
    public class EventTypeRepository : Repository<Eventtype>, IEventTypeRepository
    {
        public EventTypeRepository(membermaxContext context)
           : base(context)
        { }

        public async Task<IEnumerable<Eventtype>> GetEventtypelookup()
        {
            return await membermaxContext.Eventtypes
                .ToListAsync();
        }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }
    }
}
