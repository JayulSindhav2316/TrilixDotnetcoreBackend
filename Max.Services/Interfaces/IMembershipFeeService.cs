using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;
using Max.Core.Models;

namespace Max.Services.Interfaces
{
    public interface IMembershipFeeService
    {
        Task<IEnumerable<Membershipfee>> GetAllMembershipFees();
        Task<IEnumerable<MembershipFeeModel>> GetMembershipFeesByMembershipTypeId(int id);
        Task<IEnumerable<MembershipFeeModel>> GetMembershipFeesByFeeIds(string feeIds);
        Task<Membershipfee> GetMembershipFeeById(int id);
        Task<Membershipfee> CreateMembershipFee(MembershipFeeModel membershipFeeModel);
        Task<Membershipfee> UpdateMembershipFee(MembershipFeeModel membershipFeeModel);
        Task<bool> DeleteMembershipFee(int membershipTypeId);
    }
}