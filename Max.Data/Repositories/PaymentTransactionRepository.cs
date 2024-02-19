using System;
using System.Collections.Generic;
using System.Text;
using Max.Data.DataModel;
using Max.Data.Interfaces;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Max.Core.Helpers;
using Max.Core;
using static Max.Core.Constants;

namespace Max.Data.Repositories
{
    public class PaymentTransactionRepository : Repository<Paymenttransaction>, IPaymentTransactionRepository
    {
        public PaymentTransactionRepository(membermaxContext context)
           : base(context)
        { }
        public async Task<Paymenttransaction> GetPaymentTransactionByIdAsync(int paymentTransactionId) 
        {
            return await membermaxContext.Paymenttransactions.Where(p => p.PaymentTransactionId == paymentTransactionId).SingleOrDefaultAsync();
        }
        public async Task<IEnumerable<Paymenttransaction>> GetPaymentTransactionsByDateAsync(DateTime transactionDate)
        {
            return await membermaxContext.Paymenttransactions.Where(p => p.TransactionDate == transactionDate).ToListAsync();
        }
        public async Task<IEnumerable<Paymenttransaction>> GetPaymentTransactionsByEntityIdAsync(int id)
        {
            return await membermaxContext.Paymenttransactions.Where(p => p.EntityId == id).ToListAsync();
        }
        public async Task<IEnumerable<Paymenttransaction>> GetPaymentTransactionsByReceiptIdAsync(int receiptId)
        {
            return await membermaxContext.Paymenttransactions.Where(p => p.ReceiptId == receiptId).ToListAsync();
        }
        public async Task<IEnumerable<Paymenttransaction>> GetPaymentTransactionsByTransactionTypeAsync(int transactionType)
        {
            return await membermaxContext.Paymenttransactions.Where(p => p.TransactionType == transactionType).ToListAsync();
        }
        public async Task<IEnumerable<Paymenttransaction>> GetPaymentTransactionsByPaymentTypeAsync(string paymentType)
        {
            return await membermaxContext.Paymenttransactions.Where(p => p.PaymentType == paymentType).ToListAsync();
        }
        public async Task<IEnumerable<Paymenttransaction>> GetCreditCardPaymentsByDateRangeAsync(string cardType, DateTime fromDate, DateTime toDate)
        {
            //Build predicate 
            var predicate = PredicateBuilder.True<Paymenttransaction>();
            
            predicate = predicate.And(x => x.PaymentType == "CreditCard" || x.PaymentType== "eCheck");

            if (!cardType.ToUpper().Contains("ALL"))
            {
                predicate = predicate.And(x => x.CardType == cardType);
            }
            if (fromDate == toDate)
            {
                predicate = predicate.And(x => x.TransactionDate.Date == fromDate.Date);
            }
            if (fromDate < toDate)
            {
                predicate = predicate.And(x => x.TransactionDate.Date >= fromDate.Date && x.TransactionDate.Date <= toDate.Date);
            }
            return await membermaxContext.Paymenttransactions
                        .Where(predicate)
                        .Include(x => x.Receipt)
                        .Include(x => x.Entity)
                        .AsNoTracking()
                        .OrderBy(x => x.TransactionDate).ThenBy(x => x.Receipt.Receiptid)
                        .ToListAsync();
        }
        public async Task<IEnumerable<Paymenttransaction>> GetDepositsByDateRangeAsync(string paymentTypes, DateTime fromDate, DateTime toDate, string portal)
        {
            //Build predicate 
            var predicate = PredicateBuilder.True<Paymenttransaction>();
            
            predicate = predicate.And(x => x.Status == (int)PaymentTransactionStatus.Approved);
            predicate = predicate.And(x => x.PaymentType != "Member Credit");
            if (!paymentTypes.ToUpper().Contains("ALL"))
            {
                var modes = paymentTypes.Split(",");
                predicate = predicate.And(x => modes.Contains(x.PaymentType));
            }
            if (fromDate == toDate)
            {
                predicate = predicate.And(x => x.TransactionDate.Date == fromDate.Date);
            }
            if (fromDate < toDate)
            {
                predicate = predicate.And(x => x.TransactionDate.Date >= fromDate.Date && x.TransactionDate.Date <= toDate.Date);
            }
            if(portal.ToUpper() != "ALL")
            {
                if(portal =="Staff")
                {
                    predicate = predicate.And(x => x.Receipt.Portal ==(int)Portal.StaffPortal);
                }
                else
                {
                    predicate = predicate.And(x => x.Receipt.Portal == (int)Portal.MemberPortal);
                }
            }
            return await membermaxContext.Paymenttransactions
                        .Where(predicate)
                        .Include(x => x.Receipt)
                            .ThenInclude(x => x.Staff)
                        .Include(x => x.Entity)
                        .AsNoTracking()
                        .OrderBy(x => x.TransactionDate).ThenBy(x => x.Receipt.Receiptid)
                        .ToListAsync();
        }
        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }

    }
}
