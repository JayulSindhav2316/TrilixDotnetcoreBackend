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
    public class PaperInvoiceRepository : Repository<Paperinvoice>, IPaperInvoiceRepository
    {
        public PaperInvoiceRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<IEnumerable<Paperinvoice>> GetAllPaperInvoicesAsync()
        {
            return await membermaxContext.Paperinvoices
                .ToListAsync();
        }

        public async Task<Paperinvoice> GetPaperInvoiceByIdAsync(int id)
        {
            return await membermaxContext.Paperinvoices
                .SingleOrDefaultAsync(m => m.PaperInvoiceId == id);
        }
        public async Task<IEnumerable<Paperinvoice>> GetPaperInvoicesWithInvoicesByCycleId(int id)
        {
            var paperInvoices = await membermaxContext.Paperinvoices
                                 .Include(x => x.Invoice)
                                 .Include(x => x.Entity)
                                 .Where(x => x.PaperBillingCycleId == id)
                                 .AsNoTracking()
                                 .ToListAsync();

            return paperInvoices;
        }

        public async Task<IEnumerable<Paperinvoice>> GetPaperInvoicesByCycleId(int id)
        {
            var paperInvoices = await membermaxContext.Paperinvoices
                                 .Where(x => x.PaperBillingCycleId == id)
                                 .ToListAsync();

            return paperInvoices;
        }

        public async Task<IEnumerable<Paperinvoice>> GetAllPaperInvoicesByCycleId(int cycleId)
        {
            var paperInvoices = await membermaxContext.Paperinvoices
                                 .Where(x => x.PaperBillingCycleId == cycleId)
                                 .ToListAsync();

            return paperInvoices;
        }
        public async Task<IEnumerable<Paperinvoice>> GetPaperInvoiceDetailsByCycleId(int id)
        {
            var paperInvoices = await membermaxContext.Paperinvoices
                                    .Include(x => x.Invoice)
                                        .ThenInclude(x => x.Entity)
                                    .Include(x => x.Invoice)
                                        .ThenInclude(x => x.Invoicedetails)
                                    .Include(x => x.Entity.People)
                                    .Include(x => x.Entity.Companies)
                                    .Include(x => x.PaperBillingCycle)
                                    .Include(x => x.Invoice.Entity.People)
                                    .Include(x => x.Invoice.Entity.Companies)
                                    .Include(x => x.Invoice.Membership)
                                    .Include(x => x.Invoice.Membership.MembershipType)
                                       .ThenInclude(x => x.PeriodNavigation)
                                    .Include(x => x.Invoice.Membership.Membershipconnections)
                                    .Where(x => x.PaperBillingCycleId == id && x.Status != (int)PaperInvoiceStatus.Deleted)
                                    .AsNoTracking()
                                    .ToListAsync();

            return paperInvoices;
        }
        public async Task<Paperinvoice> GetPaperInvoiceDetailsByInvoiceId(int id)
        {
            var paperInvoice = await membermaxContext.Paperinvoices
                                    .Include(x => x.Invoice)
                                        .ThenInclude(x => x.Entity)
                                    .Include(x => x.Entity)
                                    .Include(x => x.PaperBillingCycle)
                                    .Where(x => x.PaperInvoiceId == id)
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync();

            return paperInvoice;
        }

        public async Task<IEnumerable<Paperinvoice>> GetPaperPrliminaryInvoices()
        {
            var paperInvoices = await membermaxContext.Paperinvoices
                                 .Include(x => x.Invoice)
                                 .Include(x => x.Entity.People)
                                 .Include(x => x.Entity.Companies)
                                 .Include(x => x.Invoice.Entity.People)
                                 .Include(x => x.Invoice.Entity.Companies)
                                 .Include(x => x.PaperBillingCycle)
                                 .Where(x => x.PaperBillingCycle.Status == (int)BillingStatus.Generated)
                                 .AsNoTracking()
                                 .ToListAsync();

            return paperInvoices;
        }

        public async Task<IEnumerable<Paperinvoice>> GetLastManualBillingDraftsAsync(int billingCycleId)
        {
            var paperInvoices = await membermaxContext.Paperinvoices
                                 .Include(x => x.Invoice)
                                 .Include(x => x.Entity)
                                 //.ThenInclude(t => t.Memberships)
                                 //    .ThenInclude(x => x.MembershipType)
                                 //        .ThenInclude(y => y.CategoryNavigation)
                                 .Include(x => x.PaperBillingCycle)
                                 .Where(x => x.PaperBillingCycleId == billingCycleId)
                                 .AsNoTracking()
                                 .ToListAsync();

            return paperInvoices;
        }
        public async Task<IEnumerable<Paperinvoice>> GetPaperInvoicesForFinalizationByCycleId(int billingCycleId)
        {
            var paperInvoices = await membermaxContext.Paperinvoices
                                 .Include(x => x.Invoice)
                                    .ThenInclude(x => x.Invoicedetails)
                                 .Include(x => x.PaperBillingCycle)
                                 .Include(x => x.Entity)
                                 .Where(x => x.PaperBillingCycleId == billingCycleId
                                               && (x.Status == (int)PaperInvoiceStatus.Inactive || x.Status == (int)PaperInvoiceStatus.Edited))
                                 .ToListAsync();

            return paperInvoices;
        }

        public async Task<IEnumerable<Paperinvoice>> GetPaperInvoicesForFinalizationByBatchId(int batchId)
        {
            var paperInvoices = await membermaxContext.Paperinvoices
                                 .Include(x => x.Invoice)
                                    .ThenInclude(x => x.Invoicedetails)
                                 .Include(x => x.PaperBillingCycle)
                                    .ThenInclude(x => x.Billingbatches)
                                 .Where(x => x.PaperBillingCycle.Billingbatches.Any(x => x.BillingBatchId == batchId)
                                               && x.Status == (int)InvoiceStatus.Draft
                                               && x.Invoice.Status == (int)InvoiceStatus.Draft)
                                 .ToListAsync();

            return paperInvoices;
        }

        public async Task<Paperinvoice> GetDeletedPaperInvoicesByMembershipId(int membershipId, int cycleId)
        {
            return await membermaxContext.Paperinvoices
                   .Where(x => x.Invoice.MembershipId == membershipId
                            && x.PaperBillingCycleId == cycleId
                            && x.Status == (int)PaperInvoiceStatus.Deleted)
                   .FirstOrDefaultAsync();
        }
        public async Task<Paperinvoice> GetDeletedOrEditedPaperInvoicesByMembershipId(int membershipId, int cycleId)
        {
            return await membermaxContext.Paperinvoices
                   .Where(x => x.Invoice.MembershipId == membershipId
                            && x.PaperBillingCycleId == cycleId
                            && (x.Status == (int)PaperInvoiceStatus.Deleted || x.Status == (int)PaperInvoiceStatus.Edited))
                   .FirstOrDefaultAsync();
        }

        public async Task<Paperinvoice> GetFinalizedPaperInvoicesByCycleId(int membershipId, int cycleId)
        {
            return await membermaxContext.Paperinvoices
                   .Where(x => x.Invoice.MembershipId == membershipId
                            && x.PaperBillingCycleId == cycleId
                            && x.Invoice.Status == (int)InvoiceStatus.Finalized)
                   .FirstOrDefaultAsync();
        }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }

    }
}
