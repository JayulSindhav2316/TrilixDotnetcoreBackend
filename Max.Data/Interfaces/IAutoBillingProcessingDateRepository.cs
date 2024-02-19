using System;
using System.Collections.Generic;
using System.Text;
using Max.Data.DataModel;
using System.Threading.Tasks;

namespace Max.Data.Interfaces
{
    public interface IAutoBillingProcessingDateRepository : IRepository<Autobillingprocessingdate>
    {
        Task<Autobillingprocessingdate> GetAutoBillingProcessingDatesByABPDIdAsync(int abpdId);
        Task<IEnumerable<Autobillingprocessingdate>> GetAutoBillingProcessingDatesAsync(); 
        Task<Autobillingprocessingdate> GetAutoBillingProcessingDatesByBillingTypeAsync(string billingType);
        Task<Autobillingprocessingdate> GetAutoBillingProcessingDatesByInvoiceTypeAsync(string invoiceType);
        Task<IEnumerable<Autobillingprocessingdate>> GetAutoBillingProcessingDatesByThroughDateAsync(DateTime throughDate);
        Task<IEnumerable<Autobillingprocessingdate>> GetAutoBillingProcessingDatesByEffectiveDateAsync(DateTime effectiveDate);
        Task<IEnumerable<Autobillingprocessingdate>> GetAutoBillingProcessingDatesByStatusAsync(int status);
    }
}
