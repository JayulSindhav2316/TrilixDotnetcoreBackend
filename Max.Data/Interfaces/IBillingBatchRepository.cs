using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;

namespace Max.Data.Interfaces
{
    public interface IBillingBatchRepository : IRepository<Billingbatch>
    {
        Task<IEnumerable<Billingbatch>> GetAllBillingBatchesAsync();
        Task<Billingbatch> GetBillingBatchByIdAsync(int id);
        Task<IEnumerable<Billingbatch>> GetAllBillingBatchesByCycleIdAsync(int cycleId);
    }
}
