
using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;

namespace Max.Data.Interfaces
{
    public interface IMembershipFeeRepository : IRepository<Membershipfee>
    {
        Task<IEnumerable<Membershipfee>> GetAllMembershipFeesAsync();
        Task<Membershipfee> GetMembershipFeeByIdAsync(int id);
        Task<IEnumerable<Membershipfee>> GetMembershipFeeByMembershipTypeIdAsync(int id);
        Task<IEnumerable<Membershipfee>> GetMembershipFeeByFeeIdsAsync(int[] ids);
    }

}
