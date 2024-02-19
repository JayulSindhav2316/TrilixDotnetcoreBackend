using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Max.Data.Interfaces;
using Max.Data.Repositories;
using Max.Data.DataModel;
using Max.Core;

namespace Max.Data.Repositories
{
    public class InvoiceDetailRepository : Repository<Invoicedetail>, IInvoiceDetailRepository
    {
        public InvoiceDetailRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<IEnumerable<Invoicedetail>> GetAllInvoiceDetailsAsync()
        {
            return await membermaxContext.Invoicedetails
                .ToListAsync();
        }

        public async Task<Invoicedetail> GetInvoiceDetailByIdAsync(int id)
        {
            return await membermaxContext.Invoicedetails
                .Include(x => x.Receiptdetails)
                .Include(x => x.Writeoffs)
                .SingleOrDefaultAsync(m => m.InvoiceDetailId == id);
        }

        public async Task<IEnumerable<Invoicedetail>> GetAllInvoiceDetailsByEntityIdAsync(int entityId)
        {
            return await membermaxContext.Invoicedetails
                .Include(x => x.Invoice)
                .Where(m => m.Invoice.EntityId == entityId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Invoicedetail>> GetAllInvoiceDetailsByInvoiceIdAsync(int invoiceId)
        {
            return await membermaxContext.Invoicedetails
              .Include(x => x.Invoice)
              .Where(m => m.Invoice.InvoiceId == invoiceId)
              .AsNoTracking()
              .ToListAsync();
        }

        public async Task<IEnumerable<Invoicedetail>> GetInvoiceDetailsByReceiptId(int receiptId)
        {
            return await membermaxContext.Invoicedetails
               .Include(x => x.Invoice)
               .Where(x => x.Receiptdetails.Any(x => x.ReceiptHeaderId == receiptId))
               .ToListAsync();
        }

        public decimal GetInvoiceItemBalanceById(int id)
        {
            return membermaxContext.Invoicedetails.
               Include(x => x.Receiptdetails)
               .Include(x => x.Writeoffs)
               .Where(x => x.InvoiceDetailId == id)
              .AsNoTracking()
              .Select(x => x.Amount - x.Writeoffs.Sum(x => x.Amount ?? 0) - x.Receiptdetails.Where(x => x.Status != (int)ReceiptStatus.Created && x.Status != (int)ReceiptStatus.Void).Select(x => x.Amount).Sum()).Sum();

        }
        public async Task<IEnumerable<Invoicedetail>> GetInvoiceDetailsByBillableEntityIdAsync(int entityId)
        {
            return await membermaxContext.Invoicedetails
                .Where(x => x.BillableEntityId == entityId)
                .Include(x => x.Invoice)
                    .ThenInclude(x => x.Entity)
                .Include(x => x.Invoice)
                      .ThenInclude(x => x.BillableEntity)
                .Include(x => x.Receiptdetails)
                    .ThenInclude(x => x.ReceiptHeader)
                 .Include(x => x.Receiptdetails)
                    .ThenInclude(x => x.Refunddetails)
                 .Include(x => x.Writeoffs)
                    .ThenInclude(x => x.User)
               .Include(x => x.Invoice)
                    .ThenInclude(x => x.Membership)
                      .ThenInclude(x => x.MembershipType)
               .Include(x => x.Invoice)
                   .ThenInclude(x => x.Paperinvoices)
                .ToListAsync();
        }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }

    }
}
