using System;
using System.Collections.Generic;
using System.Text;
using Max.Data.DataModel;
using System.Threading.Tasks;

namespace Max.Data.Interfaces
{
    public interface IJournalEntryDetailRepository : IRepository<Journalentrydetail>
    {
        Task<Journalentrydetail> GetJournalEntryDetailByIdAsync(int id);
        Task<IEnumerable<Journalentrydetail>> GetJoutnalEntriesByJournalIdAsync(int id);
        Task<IEnumerable<Journalentrydetail>> GetReceiptsByDateRangeAsync(string glAccount, DateTime fromDate, DateTime toDate);
    }
}
