using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;
using Max.Core.Models;
using System;

namespace Max.Services.Interfaces
{
    public interface IMembershipService
    {
        Task<IEnumerable<Membership>> GetAllMemberships();
        Task<MembershipModel> GetMembershipById(int id);
        Task<Membership> CreateMembership(MembershipModel model);
        Task<IEnumerable<Membership>> GetAllMembershipDueByThroughDate(int billingType, DateTime throughDate);
        Task<IEnumerable<Membership>> GetMembershipDuesByMembershipTypeAndThroughDate(int membershipType, DateTime throughDate);
        Task<MembershipModel> CreateNewMembership(MembershipSessionModel model);
        Task<bool> CancelNewMembership(MembershipCancelModel model);
        Task<Membership> UpdateMembership(MembershipModel model);
        Task<Membership> UpdateMembershipDetails(MembershipEditModel model);
        Task<bool> UpdateNextBillDate(int membershipId);
        Task<bool> UpdateNextBillDate(int membershipId, string date);
        Task<bool> DeleteMembership(int invoiceId);
        Task<DateTime> GetMembershipEndDate(int periodId, DateTime startDate);
        Task<bool> TerminateMembership(MembershipChangeModel model);
        Task<DoughnutChartModel> GetMembershipTerminationsByType();
        Task<PieChartModel> GetMembershipsByType();
        Task<BarChartModel> GetMembershipExpirationData();
        Task<bool> RenewMembership(int membershipId);
        Task<IEnumerable<Membership>> GetMembershipRenewalsDuesByMembershipTypeAsync(int membershipType, DateTime throughDate);

    }
}
