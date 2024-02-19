using Max.Data.DataModel;
using Max.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Max.Data.Repositories
{
    public class ClientLogRepository : Repository<Clientlog>, IClientLogRepository
    {
        public ClientLogRepository(membermaxContext context)
           : base(context)
        { }
        public async Task<IEnumerable<Clientlog>> GetAllLientLogsAsync()
        {
            return await membermaxContext.Clientlogs
               .AsNoTracking()
               .ToListAsync();
        }
        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }

    }
}
