using Max.Data.Audit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Max.Data.Interfaces
{
    public interface IAuditHistoryRepository : IRepository<AuditMetaDataEntity>
    {
        Task<AuditMetaDataEntity> GetAuditHistoryByIdAsync(string primaryKey);
        Task<List<AuditEntity>> GetDeletedAuditHistoryByIdAsync(string primaryKey);
        Task<List<AuditEntity>> GetAddedAuditHistoryByIdAsync(string primaryKey);
        Task<List<AuditEntity>> GetModifiedAuditHistoryByIdAsync(string primaryKey);
    }
}
