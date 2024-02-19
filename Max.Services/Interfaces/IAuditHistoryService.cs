using Max.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Max.Services.Interfaces
{
    public interface IAuditHistoryService
    {
        Task<List<AuditHistoryModel>> GetAuditHistoryById(string primaryKey);
    }
}
