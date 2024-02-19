
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;

namespace Max.Data.Interfaces
{

    public interface IMembershipPeriodRepository : IRepository<Membershipperiod>
    {
        Task<IEnumerable<Membershipperiod>> GetAllMembershipPeriodsAsync();
        Task<Membershipperiod> GetMembershipPeriodByIdAsync(int id);
        Task<DateTime> GetMembershipEndDateByIdAsync(int periodId, DateTime startDate);
    }
}
