using System;
using System.Collections.Generic;
using System.Text;
using Max.Data.DataModel;
using Max.Data.Interfaces;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Max.Core.Helpers;

namespace Max.Data.Repositories
{
    public class JournalEntryDetailRepository : Repository<Journalentrydetail>, IJournalEntryDetailRepository
    {
        public JournalEntryDetailRepository(membermaxContext context)
           : base(context)
        { }

        public async Task<Journalentrydetail> GetJournalEntryDetailByIdAsync(int id)
        {
            return await membermaxContext.Journalentrydetails.Where(r => r.JournalEntryDetailId == id).SingleOrDefaultAsync();
        }
        public async Task<IEnumerable<Journalentrydetail>> GetJoutnalEntriesByJournalIdAsync(int id)
        {
            return await membermaxContext.Journalentrydetails.Where(r => r.JournalEntryHeaderId == id).ToListAsync();
        }

        public async Task<IEnumerable<Journalentrydetail>> GetReceiptsByDateRangeAsync(string glAccount, DateTime fromDate, DateTime toDate)
        {
            //Build predicate 
            var predicate = PredicateBuilder.True<Journalentrydetail>();

            if (!glAccount.ToUpper().Contains("ALL"))
            {
                predicate = predicate.And(x => x.GlAccountCode == glAccount);
            }
            if (fromDate == toDate)
            {
                predicate = predicate.And(x => x.JournalEntryHeader.EntryDate.Date == fromDate.Date);
            }
            if (fromDate < toDate)
            {
                predicate = predicate.And(x => x.JournalEntryHeader.EntryDate.Date >= fromDate.Date && x.JournalEntryHeader.EntryDate.Date <= toDate.Date);
            }
            return await membermaxContext.Journalentrydetails
                        .Where(predicate)
                        .Include(x => x.JournalEntryHeader)
                            .ThenInclude(x => x.ReceiptHeader)
                        .Include(x => x.ReceiptDetail)
                        .AsNoTracking()
                        .OrderBy(x => x.JournalEntryHeader.EntryDate).ThenBy(x => x.JournalEntryHeader.TransactionType)
                        .ToListAsync();
        }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }
    }
}
