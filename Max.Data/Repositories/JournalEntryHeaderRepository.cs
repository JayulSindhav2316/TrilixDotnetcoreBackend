using System;
using System.Collections.Generic;
using System.Text;
using Max.Data.DataModel;
using Max.Data.Interfaces;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Max.Data.Repositories
{
    public class JournalEntryHeaderRepository : Repository<Journalentryheader>, IJournalEntryHeaderRepository
    {

        public JournalEntryHeaderRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<Journalentryheader> GetJournalEntryByIdAsync(int id)
        {
            return await membermaxContext.Journalentryheaders.SingleOrDefaultAsync(r => r.JournalEntryHeaderId == id);
        }
        public async Task<IEnumerable<Journalentryheader>> GetJournalEntriesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await membermaxContext.Journalentryheaders.ToListAsync();
        }
        public async Task<IEnumerable<Journalentryheader>> GetReceiptsByDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            return await membermaxContext.Journalentryheaders.Where(r => r.EntryDate.Date >= fromDate && r.EntryDate.Date <= toDate).ToListAsync();
        }
        public async Task<Journalentryheader> GetJournalEntriesByReceiptIdAsync(int id)
        {
            return await membermaxContext.Journalentryheaders.Where(r => r.ReceiptHeaderId == id).SingleOrDefaultAsync();
        }
        public async Task<IEnumerable<Journalentryheader>> GetJournalEntriesByStaffIdAsync(int staffId)
        {
            return await membermaxContext.Journalentryheaders.Where(r => r.UserId == staffId).ToListAsync();
        }
        public async Task<Journalentryheader> GetJournalEntryDetailsById(int id)
        {
            return await membermaxContext.Journalentryheaders.Where(r => r.JournalEntryHeaderId == id)
                        .Include(x => x.Journalentrydetails)
                        .Include(x => x.ReceiptHeader)
                        .AsNoTracking()
                        .SingleOrDefaultAsync();
        }
        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }
    }
}
