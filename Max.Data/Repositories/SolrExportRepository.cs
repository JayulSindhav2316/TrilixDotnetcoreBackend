using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Max.Data.Interfaces;
using Max.Data.Repositories;
using Max.Data.DataModel;

namespace Max.Data.Repositories
{
    public class SolrExportRepository : Repository<Solrexport>, ISolrExportRepository
    {
        public SolrExportRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<IEnumerable<Solrexport>> GetAllSolrDocumentsAsync()
        {
            return await membermaxContext.Solrexports.OrderBy(x => x.SolrId)
                .ToListAsync();
        }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }
    }
}
