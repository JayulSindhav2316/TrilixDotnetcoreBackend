using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;

namespace Max.Data.Interfaces
{
    public interface IMembershipHistoryRepository : IRepository<Membershiphistory>
    {
        Task<IEnumerable<Membershiphistory>> GetAllMembershipHistoriesAsync();
        Task<Membershiphistory> GetMembershipHistoryByIdAsync(int id);
        Task<IEnumerable<Membershiphistory>> GetAllMembershipHistoryByPersonIdAsync(int personId);
        Task<IEnumerable<Membershiphistory>> GetAllMembershipHistoryByMembershipIdAsync(int membershipId);
        Task<Membershiphistory> GetActiveMembershipHistoryByIdAsync(int id);
    }
}
