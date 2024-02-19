using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;

namespace Max.Data.Interfaces
{
    public interface IBillingCycleRepository : IRepository<Billingcycle>
    {
        Task<IEnumerable<Billingcycle>> GetAllBillingCyclesAsync();
        Task<Billingcycle> GetBillingCycleByIdAsync(int id);
        Task<Billingcycle> GetBillingCycleByNameAsync(string name);
        Task<Billingcycle> GetBillingCycleByNameAndTypeAsync(string name, int type);
        Task<int> GetLastFinalizedBillingCycleIdAsync();
        Task<IEnumerable<Billingcycle>> GetPendingCyclesAsync(int cycleType);
    }
}
