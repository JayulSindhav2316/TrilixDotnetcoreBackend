using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Max.Data.DataModel;
using Max.Core.Models;

namespace Max.Services.Interfaces
{
    public interface IAutoBillingDraftService
    {
        Task<Autobillingdraft> CreateAutoBillingDraft(AutoBillingDraftModel autoBillingDraftModel);
        Task<bool> UpdateAutoBillingDraft(AutoBillingDraftModel autoBillingDraftModel);
        Task<Autobillingdraft> GetAutobillingDraftById(int autoBillingDraftId);
        Task<List<AutoBillingDraftModel>> GetAutobillingCurrentDraft();
        Task<IEnumerable<Autobillingdraft>> GetAllAutobillingDrafts();
        Task<IEnumerable<Autobillingdraft>> GetAutobillingDraftsByPersonId(int personId);
        //Task<Autobillingdraft> GetAutobillingDraftByPaymentTransactionId(int paymentTransactionId);
        Task<IEnumerable<Autobillingdraft>> GetAutobillingDraftsByProcessStatus(int processStatus);
        Task<List<AutoBillingDraftModel>> GetAutobillingDraftsByBillingDocumentId(int billingDocumentId);
        Task<Autobillingdraft> CreateAutoBillingCreditCardDraft(BillingDocumentModel model, InvoiceModel invoice);
        Task<IEnumerable<CategoryRevenueModel>> GetAutobillingDraftsSummaryByCategoryId(int billingDocumentId);
        Task<decimal?> GetLastAutobillingDraftsAmountCreated(int billingDocumentId);
        Task<decimal?> GetLastAutobillingDraftsAmountApproved(int billingDocumentId);
        Task<decimal?> GetLastAutoBillingDraftsAmountDeclined(int billingDocumentId);
        Task<dynamic> GetLastBillingChartInvoiceChartData();
        Task<bool> SetAutoPayOnHold(AutoBillingHoldRequestModel model);
        Task<bool> ClearAutoPayOnHold(AutoBillingHoldRequestModel model);
    }
}
