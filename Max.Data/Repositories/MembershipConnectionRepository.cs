using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Max.Data.Interfaces;
using Max.Data.Repositories;
using Max.Data.DataModel;

namespace Max.Data.Repositories
{
    public class MembershipConnectionRepository : Repository<Membershipconnection>, IMembershipConnectionRepository
    {
        public MembershipConnectionRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<IEnumerable<Membershipconnection>> GetMembershipConnectionsByMembershipIdAsync(int id)
        {
            var connections = await membermaxContext.Membershipconnections
                                .Include(x => x.Entity)
                                    .ThenInclude(x => x.People)
                                .Include(x => x.Entity)
                                    .ThenInclude(x => x.Companies)
                                .Where(x => x.MembershipId == id)
                                .ToListAsync();

            return connections;
        }

        public async Task<IEnumerable<Membershipconnection>> GetMembershipConnectionsByEntityIdAsync(int id)
        {
            var connections = await membermaxContext.Membershipconnections
                                .Include(x => x.Membership)
                                .Where(x => x.EntityId == id)
                                .ToListAsync();

            return connections;
        }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }

    }
}
