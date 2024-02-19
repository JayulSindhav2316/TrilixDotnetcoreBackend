using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;
using Max.Core.Models;

namespace Max.Services.Interfaces
{
    public interface IBillingFeeService
    {        
        Task<Billingfee> CreateBillingFee(BillingFeeModel BillingfeeModel);
        Task<List<BillingFeeModel>> GetBillingFeeByMembershipId(int membershipId);
        Task<Billingfee> UpdateBillingFee(BillingFeeModel model);
    }
}
