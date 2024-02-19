using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Max.Data.DataModel;

namespace Max.Data.Interfaces
{
    public interface ISessionRegistrationGroupPricingRepository : IRepository<Sessionregistrationgrouppricing>
    {
        Task<IEnumerable<Sessionregistrationgrouppricing>> GetSessionPricingBySessionIdAsync(int sessionId);
        Task<IEnumerable<Sessionregistrationgrouppricing>> GetSessionPricingBySessionIdAndGroupIdAsync(int sessionId, int groupId);
        Task<Sessionregistrationgrouppricing> GetPricingBySessionIdGroupIdFeeIdAsync(int sessionId, int groupId, int registrationFeeTypeId);
    }
}
