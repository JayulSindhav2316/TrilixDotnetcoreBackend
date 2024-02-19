using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Max.Core.Models;
using Max.Data.DataModel;
using Max.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Max.Data.Repositories
{
    public class ReportSortorderRepository : Repository<Reportsortorder>, IReportSortorderRepository
    {
        public ReportSortorderRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<IEnumerable<Reportsortorder>> GetAllSortordersAsync()
        {
            return await membermaxContext.Reportsortorders
                .ToListAsync();
        }       

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }

    }
}
