using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;
using Max.Core.Models;

namespace Max.Services.Interfaces
{
    public interface IMembershipHistoryService
    {
        Task<IEnumerable<Membershiphistory>> GetAllMembershipHistorys();
        Task<Membershiphistory> GetMembershipHistoryById(int id);
        Task<Membershiphistory> CreateMembershipHistory(MembershipHistoryModel MembershipHistoryModel);
        Task<IEnumerable<Membershiphistory>> GetMembershipHistoryByPersonId(int personId);
        Task<Membershiphistory> UpdateMembershipHistory(MembershipHistoryModel model);
        Task<bool> DeleteMembershipHistory(int id);
    }
}
