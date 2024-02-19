using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Core.Models;
using Max.Data.DataModel;

namespace Max.Data.Interfaces
{
    public interface IMembershipRepository : IRepository<Membership>
    {
        Task<IEnumerable<Membership>> GetAllMembershipsAsync();
        Task<IEnumerable<Membership>> GetAllMembershipDueByThroughDateAsync(int billingType, DateTime throughDate);
        Task<IEnumerable<Membership>> GetMembershipDuesByMembershipTypeAsync(int membershipType, DateTime throughDate);
        Task<Membership> GetMembershipByIdAsync(int id, bool isNoTracking = true);
        Task<IEnumerable<Membership>> GetActiveMembershipByEntityIdAsync(int id);
        Task<IEnumerable<GroupDataModel>> GetMembershipsByType();
        Task<IEnumerable<GroupDataModel>> GetMembershipTerminationsByType();
        Task<int> GetMembershipExpirationsInDays(int from, int days);
        Task<IEnumerable<Membership>> GetMembershipRenewalDuesByMembershipTypeAsync(int membershipType, DateTime throughDate);
    }
}
