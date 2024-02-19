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
    public class SessionRepository : Repository<Session>, ISessionRepository
    {
        public SessionRepository(membermaxContext context)
           : base(context)
        { }

        public async Task<IEnumerable<Session>> GetAllSessionsByEventIdAsync(int eventId)
        {
            return await membermaxContext.Sessions
                .Where(x => x.EventId == eventId)
                .Include(x => x.Event)
                .OrderByDescending(x => x.SessionId)
                .ToListAsync();
        }

        public async Task<Session> GetSessionByIdAsync(int sessionId)
        {
            return await membermaxContext.Sessions
                .Include(x => x.Event)
                .Include(x => x.Questionlinks)
                .Include(x => x.Sessionleaderlinks)
                .Include(x => x.Sessionregistrationgrouppricings)
                .Where(x => x.SessionId == sessionId)
                .SingleOrDefaultAsync();
        }

        public async Task<Session> GetSessionBySessionCodeAsync(string code)
        {
            //return await membermaxContext.Sessions
            //    .Where(x => x.Code == code)
            //    .SingleOrDefaultAsync();
            return await membermaxContext.Sessions
               .FirstOrDefaultAsync(x => x.Code == code);
        }

        public async Task<Session> GetSessionByNameAsync(string name)
        {
            return await membermaxContext.Sessions
                .Where(x => x.Name == name)
                .SingleOrDefaultAsync();
        }

        public async Task<Session> GetSessionByNameAndEventIdAsync(string name, int eventId)
        {
            //return await membermaxContext.Sessions
            //    .Where(x => x.Name == name && x.EventId == eventId)
            //    .SingleOrDefaultAsync();
            return await membermaxContext.Sessions
                .FirstOrDefaultAsync(x => x.Name == name && x.EventId == eventId);
        }

        public async Task<Session> GetSessionByStartAndEndDateAsync(DateTime startDate, DateTime endDate, int eventId)
        {
            return await membermaxContext.Sessions
                .Where(x => x.StartDatetime.Value.AddSeconds(- x.StartDatetime.Value.Second) == startDate.AddSeconds(- startDate.Second) 
                && x.EndDateTime.Value.AddSeconds(-x.EndDateTime.Value.Second) == endDate.AddSeconds(-endDate.Second) && x.EventId == eventId && x.Event.EventTypeId != 3)
                .SingleOrDefaultAsync();
        }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }
    }
}

