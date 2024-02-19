using Max.Core.Models;
using Max.Data.DataModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Max.Services.Interfaces
{
    public interface IPaperInvoiceService
    {
        Task<IEnumerable<Paperinvoice>> GetPaperInvoicesWithInvoicesByCycleId(int id);
        Task<IEnumerable<PaperInvoiceModel>> GetPaperInvoicesByCycleId(int id);
        Task<List<PaperInvoiceModel>> GetPaperPrliminaryInvoices();
        Task<Paperinvoice> GetPaperInvoiceById(int id);
        Task<Paperinvoice> CreatePaperInvoice(PaperInvoiceModel model);
        Task<IEnumerable<PaperInvoiceModel>> GetLastManualBillingDrafts(int billingCycleId);
        Task<IEnumerable<InvoiceModel>> GetManualBillingInvoices(int billingCycleId);
        Task<InvoiceModel> GetManualBillingInvoiceByInvoiceId(int invoiceId);
        Task<bool> FinalizePaperInvoicesByCycleId(int id);
        Task<bool> UpdatePaperInvoice(PaperInvoiceModel model);
        Task<bool> DeletePaperInvoice(int paperInvoiceId);
        Task<IEnumerable<PaperInvoiceModel>> GetRenewalPaperInvoicesByCycleId(int id);
    }
}
