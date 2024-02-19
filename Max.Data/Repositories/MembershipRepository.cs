using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Max.Data.Interfaces;
using Max.Data.Repositories;
using Max.Data.DataModel;
using System;
using Max.Core;
using Max.Core.Models;

namespace Max.Data.Repositories
{
    public class MembershipRepository : Repository<Membership>, IMembershipRepository
    {
        public MembershipRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<IEnumerable<Membership>> GetAllMembershipsAsync()
        {
            return await membermaxContext.Memberships
                .ToListAsync();
        }

        public async Task<IEnumerable<Membership>> GetAllMembershipDueByThroughDateAsync(int billingType, DateTime throughDate)
        {
            return await membermaxContext.Memberships
                        .Where(x => x.NextBillDate.Date <= throughDate.Date && x.Status == (int)MembershipStatus.Active && x.AutoPayEnabled==(int)billingType)
                        .Include(x => x.BillableEntity)
                        .AsNoTracking()
                        .ToListAsync();
        }

        public async Task<IEnumerable<Membership>> GetMembershipDuesByMembershipTypeAsync(int membershipType, DateTime throughDate)
        {
            return await membermaxContext.Memberships
                        .Include(x => x.MembershipType)
                        .Where(x => x.NextBillDate.Date <= throughDate.Date && x.Status == (int)MembershipStatus.Active && x.MembershipTypeId == membershipType)
                        .Include(x => x.BillableEntity)
                        .AsNoTracking()
                        .ToListAsync();
        }
        public async Task<IEnumerable<Membership>> GetMembershipRenewalDuesByMembershipTypeAsync(int membershipType, DateTime throughDate)
        {
            return await membermaxContext.Memberships
                        .Include(x => x.MembershipType)
                        .Where(x => x.EndDate.Date <= throughDate.Date && x.Status == (int)MembershipStatus.Active && x.MembershipTypeId == membershipType)
                        .Include(x => x.BillableEntity)
                        .AsNoTracking()
                        .ToListAsync();
        }

        public async Task<Membership> GetMembershipByIdAsync(int id,bool isNoTracking = true)
        {
            if (isNoTracking)
            {
                return await membermaxContext.Memberships
                .Include(x => x.MembershipType)
                    .ThenInclude(x => x.CategoryNavigation)
                .Include(x => x.MembershipType)
                    .ThenInclude(x => x.PeriodNavigation)
                .Include(x => x.Billingfees)
                    .ThenInclude(x => x.MembershipFee)
                .AsNoTracking()
                .SingleOrDefaultAsync(m => m.MembershipId == id);
            }
            else
            {
                return await membermaxContext.Memberships
                .Include(x => x.MembershipType)
                    .ThenInclude(x => x.CategoryNavigation)
                .Include(x => x.MembershipType)
                    .ThenInclude(x => x.PeriodNavigation)
                .Include(x => x.Billingfees)
                    .ThenInclude(x => x.MembershipFee)                
                .SingleOrDefaultAsync(m => m.MembershipId == id);
            }
        }

        public Task<IEnumerable<Membership>> GetAllMembershipsByPersonIdAsync(int personId)
        {
            throw new System.NotImplementedException();
        }
        public async Task<IEnumerable<Membership>> GetActiveMembershipByEntityIdAsync(int entityId)
        {
            return await membermaxContext.Memberships
                .Where(x => x.BillableEntityId == entityId && x.Status == (int)MembershipStatus.Active).ToListAsync();
        }
        public async Task<IEnumerable<GroupDataModel>> GetMembershipsByType()
        {
            var data = await membermaxContext.Memberships
                       .Include(x => x.MembershipType)
                       .GroupBy(x => x.MembershipType.Name,
                                (k, m) => new GroupDataModel()
                                {
                                    GroupName = k,
                                    Value = m.Count()

                                }).ToListAsync();
            return data;
        }

        public async Task<IEnumerable<GroupDataModel>> GetMembershipTerminationsByType()
        {
            var data = await membermaxContext.Membershiphistories
                        .Where(x => x.Status==(int)MembershipStatus.Active && x.Membership.Status== (int)MembershipStatus.Terminated)
                        .Include(x => x.Membership)
                            .ThenInclude(x => x.MembershipType)
                       .GroupBy(x => x.Membership.MembershipType.Name,
                                (k, m) => new GroupDataModel()
                                {
                                    GroupName = k,
                                    Value = m.Count()

                                }).ToListAsync();
            return data;
        }

        public async Task<int> GetMembershipExpirationsInDays( int from, int to)
        {
           return await  membermaxContext.Memberships
                            .Where(x => x.EndDate.Date > DateTime.Now.Date.AddDays(from) && x.EndDate.Date  <= DateTime.Now.Date.AddDays(to)).CountAsync();
           

        }
        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }

    }
}
