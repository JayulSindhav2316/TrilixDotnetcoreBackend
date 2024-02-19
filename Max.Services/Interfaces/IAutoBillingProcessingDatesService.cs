using System;
using System.Collections.Generic;
using System.Text;
using Max.Data.Interfaces;
using Max.Services.Interfaces;
using Max.Data.DataModel;
using System.Threading.Tasks;
using Max.Core.Models;

namespace Max.Services.Interfaces
{
    public interface IAutoBillingProcessingDatesService
    {
        Task<Autobillingprocessingdate> CreateAutoBillingProcessingDate(AutoBillingProcessingDateModel autoBillingProcessingDateModel);
        Task<bool> UpdateAutoBillingProcessingDate(AutoBillingProcessingDateModel autoBillingProcessingDateModel);
        Task<Autobillingprocessingdate> GetAutoBillingProcessingDatesByABPDId(int abpdId);
        Task<IEnumerable<Autobillingprocessingdate>> GetAutoBillingProcessingDates();
        Task<Autobillingprocessingdate> GetAutoBillingProcessingDatesByBillingType(string billingType);
        Task<Autobillingprocessingdate> GetAutoBillingProcessingDatesByInvoiceType(string invoiceType);
        Task<IEnumerable<Autobillingprocessingdate>> GetAutoBillingProcessingDatesByThroughDate(DateTime throughDate);
        Task<IEnumerable<Autobillingprocessingdate>> GetAutoBillingProcessingDatesByEffectiveDate(DateTime effectiveDate);
        Task<IEnumerable<Autobillingprocessingdate>> GetAutoBillingProcessingDatesByStatus(int status);
    }
}
