using System;
using System.Collections.Generic;
using System.Text;
using Max.Data.DataModel;
using System.Threading.Tasks;
using Max.Core.Models;

namespace Max.Data.Interfaces
{
    public interface IAutoBillingDraftRepository : IRepository<Autobillingdraft>
    {
        Task<Autobillingdraft> GetAutobillingDraftByIdAsync(int autoBillingDraftId);
        Task<IEnumerable<Autobillingdraft>> GetAutobillingDraftsAsync();
        Task<IEnumerable<Autobillingdraft>> GetAutobillingDraftsByPersonIdAsync(int personId);
        Task<IEnumerable<Autobillingdraft>> GetAutobillingDraftsByProcessStatusAsync(int processStatus);
        Task<IEnumerable<Autobillingdraft>> GetAutobillingDraftsByBillingDocumentIdAsync(int billingDocumentId);
        Task<IEnumerable<Autobillingdraft>> GetAutobillingDraftsSummaryByCategoryIdAsync(int billingDocumentId);
        Task<IEnumerable<Autobillingdraft>> GetLastAutoBillingDraftPaymentsAsync(int billingDocumentId);
    }
}
