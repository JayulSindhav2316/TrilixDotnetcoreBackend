using System;
using System.Collections.Generic;
using System.Text;
using Max.Data.DataModel;
using Max.Data.Interfaces;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Max.Core.Models;
using Max.Core;

namespace Max.Data.Repositories
{
    public class ReceiptHeaderRepository : Repository<Receiptheader>, IReceiptHeaderRepository
    {

        public ReceiptHeaderRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<Receiptheader> GetReceiptByIdAsync(int receiptId)
        {
            return await membermaxContext.Receiptheaders.SingleOrDefaultAsync(r => r.Receiptid == receiptId);
        }
        public async Task<IEnumerable<Receiptheader>> GetAllReceiptsAsync()
        {
            return await membermaxContext.Receiptheaders.ToListAsync();
        }
        public async Task<IEnumerable<Receiptheader>> GetReceiptsByDateRangeAsync(DateTime fromDate, DateTime toDate) 
        {
            return await membermaxContext.Receiptheaders.Where(r => r.Date >= fromDate && r.Date <= toDate).ToListAsync();
        }
        public async Task<IEnumerable<Receiptheader>> GetReceiptsByOrganizationIdAsync(int OrganizationId)
        {
            return await membermaxContext.Receiptheaders.Where(r => r.OrganizationId == OrganizationId).ToListAsync();
        }
        public async Task<IEnumerable<Receiptheader>> GetReceiptsByStaffIdAsync(int staffId)
        {
            return await membermaxContext.Receiptheaders.Where(r => r.StaffId == staffId).ToListAsync();
        }
        public async Task<Receiptheader> GetReceiptDetailById(int id)
        {
            return await membermaxContext.Receiptheaders.Where(r => r.Receiptid == id)
                        .Include(x => x.Receiptdetails)
                            .ThenInclude(x => x.InvoiceDetail)
                                .ThenInclude(x => x.Invoice)
                        .Include(x => x.Receiptdetails)
                            .ThenInclude(x => x.Credittransactions)
                        .Include(x => x.Organization)
                        .Include(x => x.Paymenttransactions.Where(x => x.Status == (int)PaymentTransactionStatus.Approved))
                            .ThenInclude( x => x.Entity)
                        .SingleOrDefaultAsync();
        }

        public async Task<List<Receiptheader>> GetReceiptDetailsById(int[] receiptids)
        {
            return await membermaxContext.Receiptheaders.Where(r => receiptids.Contains(r.Receiptid))
                        .Include(x => x.Receiptdetails)
                            .ThenInclude(x => x.InvoiceDetail)
                                .ThenInclude(x => x.Invoice)
                        .Include(x => x.Receiptdetails)
                            .ThenInclude(x => x.Credittransactions)
                        .Include(x => x.Organization)
                        .Include(x => x.Paymenttransactions.Where(x => x.Status == (int)PaymentTransactionStatus.Approved))
                            .ThenInclude(x => x.Entity)
                        .ToListAsync();
        }

        public async Task<Receiptheader> GetReceiptItemDetailById(int id)
        {
            return await membermaxContext.Receiptheaders.Where(r => r.Receiptid == id)
                        .Include(x => x.Receiptdetails)
                        .SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<MonthDayGroupModel>> GetDailySalesByMonth()
        {
            var data = await membermaxContext.Receiptdetails
                       .Include(x => x.ReceiptHeader)
                       .Where(x => x.ReceiptHeader.Status == (int)ReceiptStatus.Active)
                       .OrderBy(x => x.ReceiptHeader.Date)
                       .GroupBy(x => new { x.ReceiptHeader.Date.Month, x.ReceiptHeader.Date.Day })
                       .Select(x => new MonthDayGroupModel()
                                {
                                    Month = x.Key.Month,
                                    Day = x.Key.Day,
                                    SaleAmount = Convert.ToInt32(x.Sum(x =>  x.Amount))

                                }).ToListAsync();
            return data;
        }

        public async Task<IEnumerable<Receiptheader>> GetReceiptsByPromoCodeIdAsync(int promoCodeId)
        {
            return await membermaxContext.Receiptheaders.Where(r => r.PromoCodeId == promoCodeId)
                      .Include(x => x.Receiptdetails)
                      .ToListAsync();
        }
        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }
    }
}
