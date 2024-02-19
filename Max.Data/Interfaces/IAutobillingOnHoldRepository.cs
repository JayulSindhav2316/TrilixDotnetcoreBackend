using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Core.Models;
using Max.Data.DataModel;

namespace Max.Data.Interfaces
{
    public interface IAutoBillingOnHoldRepository : IRepository<Autobillingonhold>
    {
        Task<Autobillingonhold> GetAutoBillingOnHoldByMembershipIdAsync(int membershipId);

    }
}
