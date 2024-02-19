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
    public class ReceiptDetailRepository : Repository<Receiptdetail>, IReceiptDetailRepository
    {
        public ReceiptDetailRepository(membermaxContext context)
           : base(context)
        { }

        public async Task<Receiptdetail> GetReceiptDetailsByIdAsync(int receiptDetailid)
        {
            return await membermaxContext.Receiptdetails
                .Include(x => x.ReceiptHeader)
                .Include(x => x.InvoiceDetail)
                    .ThenInclude(x => x.Invoice)
                .Where(r => r.ReceiptDetailId == receiptDetailid).SingleOrDefaultAsync();
        }
        public async Task<IEnumerable<Receiptdetail>> GetReceiptDetailsByReceiptIdAsync(int receiptId)
        {
            return await membermaxContext.Receiptdetails
                .Include(x => x.ReceiptHeader)
                .Include(x => x.InvoiceDetail)
                    .ThenInclude(x => x.Invoice)
                .Include(x => x.InvoiceDetail)
                    .ThenInclude(x => x.Invoice)
                        .ThenInclude(x => x.Membership)
                .Where(r => r.ReceiptHeaderId == receiptId)
                .OrderBy(x => x.InvoiceDetail.InvoiceId)
                .ThenBy(x => x.InvoiceDetailId)
                .ToListAsync();
        }
        public async Task<IEnumerable<Receiptdetail>> GetReceiptDetailsByInvoiceIdAsync(int invoiceId)
        {
            return await membermaxContext.Receiptdetails.Where(r => r.InvoiceDetail.InvoiceId == invoiceId).ToListAsync();
        }
        public async Task<IEnumerable<Receiptdetail>> GetReceiptDetailsByInvoiceDetailIdAsync(int invoiceDetailId)
        {
            return await membermaxContext.Receiptdetails.Where(r => r.InvoiceDetailId == invoiceDetailId).ToListAsync();
        }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }
    }
}
