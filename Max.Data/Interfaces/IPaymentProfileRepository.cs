using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;

namespace Max.Data.Interfaces
{
    public interface IPaymentProfileRepository : IRepository<Paymentprofile>
    {
        Task<IEnumerable<Paymentprofile>> GetAllPaymentProfilesAsync();
        Task<Paymentprofile> GetPaymentProfileByIdAsync(int id);
        Task<IEnumerable<Paymentprofile>> GetPaymentProfileByEntityIdAsync(int id );
        Task<IEnumerable<Paymentprofile>> GetActivePaymentProfileByEntityIdAsync(int id);
        Task<Paymentprofile> GetPreferredPaymentProfileByEntityIdAsync(int id);
        Task<Paymentprofile> GetAutoBillingPaymentProfileByEntityIdAsync(int id);
    }
}