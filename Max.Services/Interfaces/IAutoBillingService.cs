using Max.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Max.Services.Interfaces
{
    public interface IAutoBillingService
    {
        Task<bool> IsAutoBillingJobDue();

        Task<AutoBillingJobModel> CreatAutoBillingJob();
        Task<AutoBillingJobModel> GetNextAutoBillingJob();
        Task<bool> UpdateJobStatus(int jobId, int staus);
        Task<bool> RegenrateAutoBillingDraft(int billingDocumentId);
       
    }
}
