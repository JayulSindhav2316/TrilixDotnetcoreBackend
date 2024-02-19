using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Max.Data.Interfaces;
using Max.Data.Repositories;
using Max.Data.DataModel;

namespace Max.Data.Repositories
{
    public class CommunicationRepository : Repository<Communication>, ICommunicationRepository
    {
        public CommunicationRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<IEnumerable<Communication>> GetAllCommunicationsAsync()
        {
            return await membermaxContext.Communications
                .OrderByDescending(x => x.Date)
                .ToListAsync();
        }

        public async Task<Communication> GetCommunicationByIdAsync(int id)
        {
            return await membermaxContext.Communications
                .SingleOrDefaultAsync(m => m.CommunicationId == id);
        }

        public async Task<IEnumerable<Communication>> GetAllCommunicationsByEntityIdAsync(int entityId)
        {
            return await membermaxContext.Communications
                .Where(x  => x.EntityId== entityId)
                .OrderByDescending(x=>x.Date)
               .ToListAsync();
        }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }

    }
}
