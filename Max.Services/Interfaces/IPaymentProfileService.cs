using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;
using Max.Core.Models;

namespace Max.Services.Interfaces
{
    public interface IPaymentProfileService
    {
        Task<IEnumerable<Paymentprofile>> GetAllPaymentProfiles();
        Task<PaymentProfileModel> GetPaymentProfileById(int id);
        Task<IEnumerable<Paymentprofile>> GetPaymentProfileByEntityId(int id);
        Task<Paymentprofile> CreatePaymentProfile(PaymentProfileModel model);
        Task<PaymentProfileModel> UpdatePaymentProfile(PaymentProfileModel model);
        Task<bool> DeletePaymentProfile(int id);
        Task<bool> SetPreferredPaymentMethod(int entityId, int paymentProfileId);
        Task<bool> SetAutoBillingPaymentMethod(int entityId, int paymentProfileId);
        Task<bool> DeletePaymentProfile(int entityId, int paymentProfileId);
    }
}