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
    public class SessionLeaderLinkRepository : Repository<Sessionleaderlink>, ISessionLeaderLinkRepository
    {
        public SessionLeaderLinkRepository(membermaxContext context)
          : base(context)
        { }

        public async Task<IEnumerable<Sessionleaderlink>> GetSessionLeadersBySessionIdAsync(int sessionId)
        {
            return await membermaxContext.Sessionleaderlinks
                .Include(x => x.Entity)
                .Where(x => x.SessionId == sessionId)
                .ToListAsync();
        }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }
    }
}
