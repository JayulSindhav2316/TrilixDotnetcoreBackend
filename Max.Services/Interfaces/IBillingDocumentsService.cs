using Max.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Max.Services.Interfaces
{
    public interface IBillingDocumentsService
    {
        Task<BillingDocumentModel> CreateAutoBillingDocument(BillingDocumentModel model);
        Task<List<BillingDocumentModel>> GetAllBillingDocumentDetails();
        Task<bool> UpdateBillingDocumentStatus(int id, int status);
    }
}
