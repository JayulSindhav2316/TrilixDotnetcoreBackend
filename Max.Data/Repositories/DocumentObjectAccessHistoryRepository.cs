using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Max.Data.Interfaces;
using Max.Data.Repositories;
using Max.Data.DataModel;
using Max.Core.Models;

namespace Max.Data.Repositories
{
    public class DocumentObjectAccessHistoryRepository : Repository<Documentobjectaccesshistory>, IDocumentObjectAccessHistoryRepository
    {
        public DocumentObjectAccessHistoryRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<IEnumerable<Documentobjectaccesshistory>> GetAllDocumentAccessHistoryAsync()
        {
            return await membermaxContext.Documentobjectaccesshistories
                  .Include(x => x.DocumentObject)
                  .Include(x => x.Entity)
                  .ToListAsync();
        }
        public async Task<IEnumerable<Documentobjectaccesshistory>> GetDocumentAccessHistoryByDocumentObjectIdAsync(int id)
        {
            return await membermaxContext.Documentobjectaccesshistories
                .Include(x => x.DocumentObject)
                .Include(x => x.Entity)
                   .ThenInclude(x=>x.People)
                .Include(x => x.StaffUser)
                .Where(x => x.DocumentObjectId == id)
                .ToListAsync();
        }

        public async Task<IEnumerable<Documentobjectaccesshistory>> GetDocumentAccessHistoryByEntityIdAsync(int id)
        {
            return await membermaxContext.Documentobjectaccesshistories
                .Include(x => x.DocumentObject)
                .Where(x => x.EntityId == id)
                .ToListAsync();
        }

        public async Task<IEnumerable<GroupDataModel>> GetDocumentAccessHistoryByDocument()
        {
            var data = await membermaxContext.Documentobjectaccesshistories
                       .Include(x => x.DocumentObject)
                       .GroupBy(x => new {x.DocumentObject.FileName})
                       .Select(g => new GroupDataModel
                        {
                           GroupName = g.Key.FileName,
                           Value = g.Count()
                        })
                       .OrderByDescending(x => x.Value)
                       .Take(10)
                       .ToListAsync();
            return data;
        }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }

    }
}
