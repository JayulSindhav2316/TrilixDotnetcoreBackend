using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;

namespace Max.Data.Interfaces
{
    public interface IBillingFeeRepository : IRepository<Billingfee>
    {
        Task<IEnumerable<Billingfee>> GetBillingFeesByMembershipIdAsync(int membershipId);
        Task<IEnumerable<Billingfee>> GetBillingFeesDetailsByMembershipIdAsync(int membershipId);
    }
}
