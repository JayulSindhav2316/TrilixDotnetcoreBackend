using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;
using Max.Core.Models;

namespace Max.Services.Interfaces
{
    public interface IMembershipPeriodService
    {
        Task<IEnumerable<Membershipperiod>> GetAllMembershipPeriods();
        Task<Membershipperiod> GetMembershipPeriodById(int id);
        Task<Membershipperiod> CreateMembershipPeriod(MembershipPeriodModel membershipPeriodModel);
        Task<List<SelectListModel>> GetSelectList();
        Task<List<SelectListModel>> GetFrequencySelectList(int period);
    }
}
