using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Max.Data.Interfaces;
using Max.Data.Repositories;
using Max.Data.DataModel;
using System;
using Max.Core;
using Max.Core.Helpers;

namespace Max.Data.Repositories
{
    public class InvoiceRepository : Repository<Invoice>, IInvoiceRepository
    {
        public InvoiceRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<IEnumerable<Invoice>> GetAllInvoicesAsync()
        {
            return await membermaxContext.Invoices
                .Include(x => x.Invoicedetails)
                .ToListAsync();
        }

        public async Task<Invoice> GetInvoiceByIdAsync(int id)
        {
            return await membermaxContext.Invoices.AsNoTracking()
                .Include(x => x.Membership)
                    .ThenInclude(x => x.MembershipType)
                        .ThenInclude(x => x.CategoryNavigation)
                .Include(x => x.Invoicedetails)
                .Include(x => x.Entity)
                .Include(x => x.Event)
                .AsNoTracking()
                .OrderByDescending(x => x.Date)
                .SingleOrDefaultAsync(m => m.InvoiceId == id);
        }

        public async Task<Invoice> GetInvoiceSummaryByIdAsync(int id)
        {
           return await membermaxContext.Invoices.AsNoTracking()
                .Include(x => x.Invoicedetails)
                    .ThenInclude(x => x.Writeoffs)
                .Include(x => x.Membership)
                    .ThenInclude(x => x.MembershipType)
                        .ThenInclude(x => x.CategoryNavigation)                        
                .Include(x => x.BillableEntity)
                .Include(x => x.Entity)
                .Include(x=>x.Paperinvoices)
                .AsNoTracking()
                .OrderByDescending(x => x.Date)
                .SingleOrDefaultAsync(m => m.InvoiceId == id);
        }
        public async Task<Invoice> GetInvoicePaymentsByIdAsync(int id)
        {
            return await membermaxContext.Invoices.AsNoTracking()
                .Include(x => x.Entity)
                .Include(x => x.Membership)
                    .ThenInclude(x => x.MembershipType)
                        .ThenInclude(x => x.CategoryNavigation)
                .Include(x => x.Invoicedetails)
                    .ThenInclude(x => x.Receiptdetails)
                        .ThenInclude(x => x.ReceiptHeader)
                .Include(x => x.Invoicedetails)
                    .ThenInclude( x=> x.Writeoffs)
                .Include(x => x.Event)
                .AsNoTracking()
                .OrderByDescending(x => x.Date)
                .SingleOrDefaultAsync(m => m.InvoiceId == id);
        }

        public async  Task<IEnumerable<Invoice>> GetAllInvoicesByEntityIdAsync(int entityId)
        {
            return await membermaxContext.Invoices.AsNoTracking()
               .Include(x => x.Invoicedetails)
                    .ThenInclude(x => x.Receiptdetails)
                        .ThenInclude(x => x.Refunddetails)
                .Include(x => x.Invoicedetails)
                    .ThenInclude(x => x.Writeoffs)
               .AsNoTracking()
               .Where(m => m.BillableEntityId == entityId && m.Status != (int)InvoiceStatus.Draft)
               .OrderByDescending(x => x.Date)
               .ToListAsync();
        }

        public async Task<IEnumerable<Invoice>> GetInvoicesBySearchConditionAsync(int entityId, string serachBy, string item, DateTime start, DateTime end)
        {
            //Build predicate 
            var predicate = PredicateBuilder.True<Invoice>();
            predicate = predicate.And(m => m.BillableEntityId == entityId ||  m.EntityId == entityId || m.Invoicedetails.Any(x => x.BillableEntityId == entityId));
            predicate = predicate.And(m => m.Status != (int)InvoiceStatus.Draft);
            if (serachBy.ToUpper().Contains("DATE"))
            {
                if (start.Date > end.Date)
                {
                    throw new InvalidOperationException("Please select a valid Date Range.");
                }
                predicate = predicate.And(x => x.Date.Date >= start.Date && x.Date.Date <= end.Date);
            }
            if (serachBy.ToUpper().Contains("ITEM"))
            {
                if(item.IsNullOrEmpty())
                {
                    throw new InvalidOperationException("Please select a valid item description to serach for.");
                }
                predicate = predicate.And(x => x.Invoicedetails.Any(x => x.Description.Contains(item)));
            }
            
            return await membermaxContext.Invoices
               .Where(predicate)
               .Include(x => x.Invoicedetails)               
                .ThenInclude(x => x.Receiptdetails)
                    .ThenInclude(x => x.Refunddetails)
                .Include(x => x.Invoicedetails)
                .ThenInclude(x => x.Receiptdetails)
                    .ThenInclude(x => x.ReceiptHeader)
               .Include(x => x.Membership)
                .ThenInclude(x => x.MembershipType)
               .Include(x => x.Entity)
               .Include(x => x.BillableEntity)
               .Include(x => x.Invoicedetails)
                .ThenInclude(x => x.Writeoffs)
                    .ThenInclude(x => x.User)
               .Include(x => x.Paperinvoices)
               .AsNoTracking()
               .OrderByDescending(x => x.Date)
               .ToListAsync();
        }
        public decimal GetInvoiceBalanceByEntityId(int entityId)
        {
            return  membermaxContext.Invoicedetails.
               Include(x => x.Receiptdetails)
               .Include(x => x.Writeoffs)
               .Include( x => x.Invoice)
               .Where(x => (x.Invoice.EntityId == entityId || x.Invoice.BillableEntityId == entityId) && x.Status != (int)InvoiceStatus.Draft)
              .AsNoTracking()
              .Select(x => x.Amount  - x.Writeoffs.Sum(x => x.Amount??0)- x.Receiptdetails.Where(x => x.Status!= (int) ReceiptStatus.Created && x.Status != (int)ReceiptStatus.Void).Select(x => x.Amount).Sum()).Sum();
              
        }

        public async Task<Invoice> GetInvoicePrintDetailsByIdAsync(int invocieId)
        {
            return await membermaxContext.Invoices.AsNoTracking()
                .Include(x => x.Invoicedetails)
                .ThenInclude(x => x.Writeoffs).AsNoTracking()
                
                .Include(x => x.Entity)
                    .ThenInclude(x => x.Organization)
                .Include(x => x.BillableEntity)
                    .ThenInclude(x => x.People)
                .Include(x => x.BillableEntity)
                    .ThenInclude(x => x.Companies)
                .Include(x => x.Membership)
                    .ThenInclude(x => x.MembershipType)
                        .ThenInclude(x => x.CategoryNavigation)
                .Include(x=>x.Event)
                .SingleOrDefaultAsync(m => m.InvoiceId == invocieId);
        }

        public async Task<IEnumerable<Invoice>> GetMembershipInvoicePayments()
        {
            return  await membermaxContext.Invoices.AsNoTracking()
               .Include(x => x.Invoicedetails)
                    .ThenInclude(x => x.Receiptdetails)
               .Include(x => x.BillableEntity)
               .Include(x => x.Membership)
                   .ThenInclude(x => x.MembershipType)
               .ToListAsync();
        }

        public async Task<IEnumerable<Invoice>> GetAllActiveInvoicesWithDuesAsync()
        {
            return await membermaxContext.Invoices.AsNoTracking()
                .Include(x => x.Entity)
                .Include(x => x.Entity.People)
                .Include(x => x.Entity.Companies)
                .Include(x => x.Invoicedetails)
                    .ThenInclude(x => x.Receiptdetails)
                .Include(x => x.BillableEntity)
                .Include(x => x.BillableEntity.People)
                .Include(x => x.BillableEntity.Companies)
                .Include(x => x.Membership)
                    .ThenInclude(x => x.MembershipType)
                .Include(x => x.PaymentTransaction)
                .Where(x => x.Status == (int)InvoiceStatus.Finalized)
                .ToListAsync();
        }

        public async Task<IEnumerable<Invoice>> GetInvoicesByReceiptId(int receiptId)
        {
            return await membermaxContext.Invoices.AsNoTracking()
               .Include(x => x.Invoicedetails)
                    .ThenInclude(x => x.Receiptdetails)
                        .ThenInclude(x => x.ReceiptHeader)
               .Include(x => x.Membership)
                   .ThenInclude(x => x.MembershipType)
               .Where(x => x.PaymentTransaction.ReceiptId==receiptId)
               .ToListAsync();
        }

        public async Task<Invoice> GetFinalizedInvoicesByMembershipId(int membershipId, DateTime nextBilldate)
        {
            return await membermaxContext.Invoices
                   .Where(x => x.MembershipId == membershipId 
                            && x.DueDate.Date == nextBilldate.Date
                            && x.Status == (int)InvoiceStatus.Finalized)
                   .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Invoice>> GetInvoicesByMultipleInvoiceIdsAsync(int[] invoiceIds)
        {
            return await membermaxContext.Invoices.AsNoTracking()
                .Where(x => invoiceIds.Contains(x.InvoiceId))
                .Include(x => x.Membership)
                    .ThenInclude(x => x.MembershipType)
                        .ThenInclude(x => x.CategoryNavigation)
                .Include(x => x.Invoicedetails)
                .AsNoTracking()
                .OrderByDescending(x => x.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<Invoice>> GetInvoicesWithBalanceByEntityIdAsync(int entityId)
        {
            return await membermaxContext.Invoices
                .Include(x => x.Invoicedetails)
                .Where(x => x.BillableEntityId == entityId && x.Status != (int)InvoiceStatus.FullyPaid && x.Status != (int)InvoiceStatus.Draft)
                .ToListAsync();
        }

        public async Task<IEnumerable<Invoice>> GetAllInvoicesByEventIdAsync(int eventId)
        {
            return await membermaxContext.Invoices.AsNoTracking()
              .Include(x => x.Invoicedetails)
                   .ThenInclude(x => x.Receiptdetails)
              .AsNoTracking()
              .Where(m => m.EventId == eventId)
              .OrderByDescending(x => x.Date)
              .ToListAsync();
        }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }

    }
}
