using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Max.Data.DataModel;
using Max.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Max.Data.Repositories
{
    public class SessionRegistrationGroupPricingRepository : Repository<Sessionregistrationgrouppricing>, ISessionRegistrationGroupPricingRepository
    {
        public SessionRegistrationGroupPricingRepository(membermaxContext context)
         : base(context)
        { }

        public async Task<IEnumerable<Sessionregistrationgrouppricing>> GetSessionPricingBySessionIdAndGroupIdAsync(int sessionId, int groupId)
        {
            return await membermaxContext.Sessionregistrationgrouppricings
                .Where(x => x.SessionId == sessionId && x.RegistrationGroupId == groupId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Sessionregistrationgrouppricing>> GetSessionPricingBySessionIdAsync(int sessionId)
        {
            return await membermaxContext.Sessionregistrationgrouppricings.Include(x => x.RegistrationGroup)
                .Where(x => x.SessionId == sessionId)
                .ToListAsync();
        }

        public async Task<Sessionregistrationgrouppricing> GetPricingBySessionIdGroupIdFeeIdAsync(int sessionId, int groupId, int registrationFeeTypeId)
        {
            return await membermaxContext.Sessionregistrationgrouppricings
                .Where(x => x.SessionId == sessionId && x.RegistrationGroupId == groupId && x.RegistrationFeeTypeId == registrationFeeTypeId)
                .SingleOrDefaultAsync();
        }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }
    }
}
