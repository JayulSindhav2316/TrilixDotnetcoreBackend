using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Max.Core.Models;
using Max.Data.DataModel;
using Max.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Max.Data.Repositories
{
    public class ReportFilterRepository : Repository<Reportfilter>, IReportFilterRepository
    {
        public ReportFilterRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<IEnumerable<Reportfilter>> GetAllReportFilterssAsync()
        {
            return await membermaxContext.Reportfilters.Include(r => r.Report).ToListAsync();
        }


        public async Task<Reportfilter> GetReportFilterByIdAsync(int id)
        {
            return await membermaxContext.Reportfilters.Include(r => r.Report).SingleOrDefaultAsync(m => m.ReportFilterId == id);
        }

        public async Task<IEnumerable<Reportfilter>> GetReportFiltersByReportId(int id)
        {
            return await membermaxContext.Reportfilters.Where(x => x.ReportId == id).Include(r => r.Report).ToListAsync();
        }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }

    }
}
