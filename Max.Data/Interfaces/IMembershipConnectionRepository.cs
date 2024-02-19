using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;

namespace Max.Data.Interfaces
{
    public interface IMembershipConnectionRepository : IRepository<Membershipconnection>
    {

        Task<IEnumerable<Membershipconnection>> GetMembershipConnectionsByMembershipIdAsync(int id);
        Task<IEnumerable<Membershipconnection>> GetMembershipConnectionsByEntityIdAsync(int id);
    }
}
