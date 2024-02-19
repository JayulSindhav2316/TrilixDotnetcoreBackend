using System;
using System.Collections.Generic;
using System.Text;
using Max.Data.DataModel;
using System.Threading.Tasks;

namespace Max.Data.Interfaces
{
    public interface IJournalEntryHeaderRepository : IRepository<Journalentryheader>
    {
        Task<Journalentryheader> GetJournalEntryByIdAsync(int id);
        Task<IEnumerable<Journalentryheader>> GetJournalEntriesByDateRangeAsync(DateTime fromDate, DateTime toDate);
        Task<Journalentryheader> GetJournalEntriesByReceiptIdAsync(int id);
        Task<IEnumerable<Journalentryheader>> GetJournalEntriesByStaffIdAsync(int staffId);
        Task<Journalentryheader> GetJournalEntryDetailsById(int id);
    }
}
