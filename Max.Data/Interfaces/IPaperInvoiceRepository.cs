using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;

namespace Max.Data.Interfaces
{
    public interface IPaperInvoiceRepository : IRepository<Paperinvoice>
    {
        Task<IEnumerable<Paperinvoice>> GetAllPaperInvoicesAsync();
        Task<Paperinvoice> GetPaperInvoiceByIdAsync(int id);
        Task<IEnumerable<Paperinvoice>> GetPaperInvoicesByCycleId(int id);
        Task<IEnumerable<Paperinvoice>> GetPaperInvoiceDetailsByCycleId(int id);
        Task<IEnumerable<Paperinvoice>> GetPaperInvoicesWithInvoicesByCycleId(int id);
        Task<Paperinvoice> GetPaperInvoiceDetailsByInvoiceId(int id);
        Task<IEnumerable<Paperinvoice>> GetPaperPrliminaryInvoices();
        Task<IEnumerable<Paperinvoice>> GetLastManualBillingDraftsAsync(int billingCycleId);
        Task<IEnumerable<Paperinvoice>> GetPaperInvoicesForFinalizationByCycleId(int billingCycleId);
        Task<IEnumerable<Paperinvoice>> GetPaperInvoicesForFinalizationByBatchId(int billingBatchId);
        Task<Paperinvoice> GetDeletedPaperInvoicesByMembershipId(int membershipId, int cycleId);
        Task<Paperinvoice> GetDeletedOrEditedPaperInvoicesByMembershipId(int membershipId, int cycleId);
        Task<IEnumerable<Paperinvoice>> GetAllPaperInvoicesByCycleId(int cycleId);
        Task<Paperinvoice> GetFinalizedPaperInvoicesByCycleId(int membershipId, int cycleId);
    }

}
