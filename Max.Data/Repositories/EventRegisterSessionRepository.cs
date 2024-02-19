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
    public class EventRegisterSessionRepository : Repository<Eventregistersession>, IEventRegisterSessionRepository
    {
        public EventRegisterSessionRepository(membermaxContext context)
          : base(context)
        { }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }

        public async Task<Eventregistersession> GetRegisteredSessionsByEntity(int eventId, int entityId, int sessionId)
        {
            return await membermaxContext.Eventregistersessions
                                         .Include(x => x.EventRegister)
                                         .Include(x => x.Session)
                                         .FirstOrDefaultAsync(s => s.EventRegister.EventId == eventId && s.EventRegister.EntityId == entityId && s.SessionId == sessionId);
        }

        public async Task<List<Eventregistersession>> GetRegisteredSessionsBySessionIdAsync(int sessionId)
        {
            return await membermaxContext.Eventregistersessions.Where(s => s.SessionId == sessionId).ToListAsync();
        }
    }
}
