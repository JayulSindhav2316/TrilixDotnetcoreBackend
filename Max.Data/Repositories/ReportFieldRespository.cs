using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Max.Core.Models;
using Max.Data.DataModel;
using Max.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Max.Data.Repositories
{
    public class ReportFieldRepository : Repository<Reportfield>, IReportFieldRepository
    {
        public ReportFieldRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<IEnumerable<Reportfield>> GetAllFieldsAsync()
        {
            return await membermaxContext.Reportfields
                .ToListAsync();
        }   
        
        public async Task<Reportfield> GetFieldByTitleAsync(string title)
        {
            return await membermaxContext.Reportfields
                .Where(x => x.FieldTitle== title)
               .FirstOrDefaultAsync();
        }

        public async Task<Reportfield> GetFieldByCategoryAndTitleAsync(int id, string title)
        {
            return await membermaxContext.Reportfields
                         .Where(x => x.ReportCategoryId == id && x.FieldTitle == title)
                         .FirstOrDefaultAsync();
        }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }

    }
}
