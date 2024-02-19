using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Max.Core.Models;
using Max.Data.DataModel;
using Max.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Max.Data.Repositories
{
    public class ReportShareRepository : Repository<Reportshare>, IReportShareRepository
    {
        public ReportShareRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<IEnumerable<Reportshare>> GetAllReportSharesAsync()
        {
            return await membermaxContext.Reportshares.Include(r => r.Report).ToListAsync();
        }


        public async Task<Reportshare> GetReportShareByIdAsync(int id)
        {
            return await membermaxContext.Reportshares.SingleOrDefaultAsync(m => m.ReportShareId == id);
        }

        public async Task<IEnumerable<Reportshare>> GetReportSharesByReportId(int id)
        {
            return await membermaxContext.Reportshares.Where(x => x.ReportId == id).Include(r => r.Report).ToListAsync();
        }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }

    }
}
