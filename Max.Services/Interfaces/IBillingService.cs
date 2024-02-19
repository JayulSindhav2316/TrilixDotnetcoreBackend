using Max.Core.Models;
using Max.Data.DataModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Max.Services.Interfaces
{
    public interface IBillingService
    {
        Task<bool> IsBillingJobDue();
        Task<Billingjob> CreateBillingJob(int cycleId);
        Task<BillingJobModel> GetNextBillingJob();
        Task<BillingJobModel> GetNextBillingFinalizationJob();
        Task<bool> UpdateJobStatus(int jobId, int status);
        Task<bool> UpdateCycleStatus(int cycleId, int status);
        Task<Billingcycle> CreateBillingCycle(BillingCycleModel model);
        Task<bool> FinzalizeBillingCycle(int cycleId);
        Task<Billingcycle> GetBillingCycleById(int id);
        Task<List<BillingCycleModel>> GetBillingCycles(int type);
        Task<bool> DeleteBillingCycle(int id);
        Task<bool> RegenrateBillingCycle(int id);
        Task<List<Billingemail>> GetEmailsForBillingCycle(int id);
        Task<BatchEmailNotificationModel> GetEmailNotificationDetailById(int id);
        Task<bool> UpdateBillingEmailStatus(int billingEmailId, string response, bool sent);
        Task<BillingJobModel> GetNextRenewalJob();
        Task<BillingJobModel> GetNextRenewalFinalizationJob();
        Task<List<BillingCycleNotifications>> GetBillingNotifications();
        Task<List<BillingCycleNotifications>> ClearBillingNotifications(BillingCycleNotifications model);
        Task<List<BillingCycleNotifications>> ClearAllBillingNotifications();
    }
}
