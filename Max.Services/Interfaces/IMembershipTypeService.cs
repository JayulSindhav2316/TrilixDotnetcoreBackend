using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;
using Max.Core.Models;

namespace Max.Services.Interfaces
{
    public interface IMembershipTypeService
    {
        Task<IEnumerable<MembershipTypeModel>> GetAllMembershipTypes();
        Task<MembershipTypeModel> GetMembershipTypeById(int id);
        Task<IEnumerable<MembershipTypeModel>> GetMembershipTypesByCategoryIds(string ids);
        Task<IEnumerable<SelectListModel>> GetMembershipTypeSelectListByCategoryIds(string ids);
        Task<Membershiptype> CreateMembershipType(MembershipTypeModel membershipTypeModel);
        Task<Membershiptype> UpdateMembershipType(MembershipTypeModel membershipTypeModel);
        Task<bool> DeleteMembershipType(int membershipTypeId);
    }
}