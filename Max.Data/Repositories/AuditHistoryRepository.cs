using Max.Data.Audit;
using Max.Data.DataModel;
using Max.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Max.Data.Repositories
{
    public class AuditHistoryRepository : Repository<AuditMetaDataEntity>, IAuditHistoryRepository
    {
        public AuditHistoryRepository(membermaxContext context) : base(context)
        { }
        public async Task<AuditMetaDataEntity> GetAuditHistoryByIdAsync(string primaryKey)
        {
            return await membermaxContext.AuditMetaDatas.Include(x=>x.AuditChanges).AsNoTracking().Where(x => x.ReadablePrimaryKey == primaryKey).FirstOrDefaultAsync();
        }

        public async Task<List<AuditEntity>> GetDeletedAuditHistoryByIdAsync(string primaryKey)
        {
            return await membermaxContext.Audits.Include(x=>x.AuditMetaData) .Where(x => x.OldValues !=null && x.OldValues.Contains($":{primaryKey},") && x.EntityState == EntityState.Deleted ).ToListAsync();
        }

        public async Task<List<AuditEntity>> GetAddedAuditHistoryByIdAsync(string primaryKey)
        {
            return await membermaxContext.Audits.Include(x=>x.AuditMetaData).Where(x => x.NewValues != null && x.NewValues.Contains($":{primaryKey},") && x.EntityState == EntityState.Added).ToListAsync();
        }
        public async Task<List<AuditEntity>> GetModifiedAuditHistoryByIdAsync(string primaryKey)
        {
            return await membermaxContext.Audits.Include(x => x.AuditMetaData).Where(x => x.OldValues != null && x.OldValues.Contains($":{primaryKey},") && x.EntityState == EntityState.Modified).ToListAsync();
        }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }
    }
}