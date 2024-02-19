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
    public class EventRepository : Repository<Event>, IEventRepository
    {
        public EventRepository(membermaxContext context)
           : base(context)
        { }

        public async Task<IEnumerable<Event>> GetAllEventsAsync()
        {
            return await membermaxContext.Events
                .Include(x => x.Sessions)
                .Include(X => X.Questionlinks)
                .Include(X => X.Linkeventgroups)
                .Include(x => x.Eventcontacts)
                .OrderByDescending(x => x.EventId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Event>> GetEventsByStatusAsync(int status)
        {
            return await membermaxContext.Events
                .Include(x => x.Sessions)
                .Include(X => X.Questionlinks)
                .Include(X => X.Linkeventgroups)
                .Include(x => x.Eventcontacts)
                .Where(x => x.Status == status)
                .OrderByDescending(x => x.EventId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Event>> GetUpcomingEventsAsync(DateTime endDate)
        {
            return await membermaxContext.Events
               .Include(x => x.Sessions)
               .Where(x => x.Status == 1 && x.ToDate > endDate || x.Status == 1 && x.ToDate == null)
               .Select(x => new Event
               {
                   EventId = x.EventId,
                   Status = x.Status,
                   Code = x.Code,
                   Name = x.Name,
                   FromDate = x.FromDate,
                   ToDate = x.ToDate,
                   Location = x.Location,
                   EventTypeId = x.EventTypeId,
               })
               .OrderByDescending(x => x.EventId)
               .ToListAsync();
        }

        public async Task<IEnumerable<Event>> GetPastEventsAsync(DateTime endDate)
        {
            return await membermaxContext.Events
                .Include(x => x.Sessions)
                .Include(X => X.Questionlinks)
                .Include(X => X.Linkeventgroups)
                .Include(x => x.Eventcontacts)
                .Where(x => x.Status == 1 && x.ToDate < endDate)
                .OrderByDescending(x => x.EventId)
                .ToListAsync();
        }

        public async Task<Event> GetEventByIdAsync(int eventId)
        {
            return await membermaxContext.Events
                .Include(x => x.Sessions)
                .Include(X => X.Questionlinks)
                .Include(X => X.Linkeventgroups)
                .Include(x => x.TimeZone)
                .Include(x => x.EventType)
                .Include(x => x.Eventcontacts)
                .Where(x => x.EventId == eventId)
                .SingleOrDefaultAsync();
        }

        public async Task<Event> GetEventByEventCodeAsync(string code)
        {
            return await membermaxContext.Events
                .Where(x => x.Code == code)
                .SingleOrDefaultAsync();
        }

        public async Task<Event> GetEventByEventNameAsync(string name)
        {
            return await membermaxContext.Events
                .Where(x => x.Name == name)
                .SingleOrDefaultAsync();
        }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }
    }
}
