using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Max.Data.Interfaces;
using Max.Data.Repositories;
using Max.Data.DataModel;
using Max.Core;
using System;

namespace Max.Data.Repositories
{
    public class CreditTransactionRepository : Repository<Credittransaction>, ICreditTransactionRepository
    {
        public CreditTransactionRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<IEnumerable<Credittransaction>> GetCreditsByEntityIdAsync(int entityId)
        {
            var credits = await membermaxContext.Credittransactions
                        .Where(x => x.EntityId == entityId)
                        .ToListAsync();

            return credits;
        }

        public async Task<decimal> GetCreditBalanceByEntityIdAsync(int entityId)
        {
            var credits = await membermaxContext.Credittransactions
                        .Where(x => x.EntityId == entityId)
                        .ToListAsync();

            if (credits != null)
            {
                if (credits.Count() > 0)
                {
                    var totalCredit = credits.Where(x => x.EntryType == (int)CreditEntryType.CreditEntry).Sum(x => x.Amount??0);
                    var totalDebit = credits.Where(x => x.EntryType == (int)CreditEntryType.DebitEntry).Sum(x => x.Amount??0);
                    return totalCredit - totalDebit;
                }
            }

            return 0;
        }
        public async Task<IEnumerable<Credittransaction>> GetCreditTransactionsByReceiptIdAsync(int receiptId)
        {
            var credits = await membermaxContext.Credittransactions
                        .Include( x=> x.ReceiptDetail)
                        .Where(x => x.ReceiptDetail.ReceiptHeaderId == receiptId)
                       .ToListAsync();

            return credits;
        }
        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }

    }
}
