using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Max.Data.Interfaces;
using Max.Data.Repositories;
using Max.Data.DataModel;

namespace Max.Data.Repositories
{
    public class MembershipFeeRepository : Repository<Membershipfee>, IMembershipFeeRepository
    {
        public MembershipFeeRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<IEnumerable<Membershipfee>> GetAllMembershipFeesAsync()
        {
            return await membermaxContext.Membershipfees
                .ToListAsync();
        }

        public async Task<Membershipfee> GetMembershipFeeByIdAsync(int id)
        {
            return await membermaxContext.Membershipfees
                .Include(x => x.Billingfees)
                .SingleOrDefaultAsync(m => m.FeeId == id);
        }

        public async Task<IEnumerable<Membershipfee>> GetMembershipFeeByFeeIdsAsync(int[] feeIds)
        {
            return await membermaxContext.Membershipfees
                .Where(x => feeIds.Contains(x.FeeId))
                .ToListAsync();
        }

        public async Task<IEnumerable<Membershipfee>> GetMembershipFeeByMembershipTypeIdAsync(int id)
        {
            return await membermaxContext.Membershipfees
                   .Where(m => m.MembershipTypeId == id)
                   .Include(x => x.GlAccount)
                   .ToListAsync();
        }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }
    }
}