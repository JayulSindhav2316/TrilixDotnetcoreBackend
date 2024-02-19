using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Max.Data.Interfaces;
using Max.Data.Repositories;
using Max.Data.DataModel;
using Max.Core;

namespace Max.Data.Repositories
{
    public class MembershipHistoryRepository : Repository<Membershiphistory>, IMembershipHistoryRepository
    {
        public MembershipHistoryRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<IEnumerable<Membershiphistory>> GetAllMembershipHistoriesAsync()
        {
            return await membermaxContext.Membershiphistories
                .ToListAsync();
        }

        public async Task<Membershiphistory> GetMembershipHistoryByIdAsync(int id)
        {
            return await membermaxContext.Membershiphistories
                .SingleOrDefaultAsync(m => m.MembershipHistoryId == id);
        }

        public async Task<Membershiphistory> GetActiveMembershipHistoryByIdAsync(int id)
        {
            return await membermaxContext.Membershiphistories
                .Where(x => x.Status == (int)MembershipStatus.Active)
                .OrderByDescending(x => x.StatusDate)
                .FirstOrDefaultAsync(m => m.MembershipId == id);
        }

        public Task<IEnumerable<Membershiphistory>> GetAllMembershipHistoryByPersonIdAsync(int personId)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<Membershiphistory>> GetAllMembershipHistoryByMembershipIdAsync(int membershipId)
        {
            throw new System.NotImplementedException();
        }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }

    }
}
