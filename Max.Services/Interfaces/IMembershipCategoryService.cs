using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;
using Max.Core.Models;

namespace Max.Services.Interfaces
{
    public interface IMembershipCategoryService
    {
        Task<IEnumerable<Membershipcategory>> GetAllMembershipCategories();
       
        Task<Membershipcategory> GetMembershipCategoryById(int id);
        Task<Membershipcategory> CreateMembershipCategory(MembershipCategoryModel membershipCategoryModel);
        Task<List<SelectListModel>> GetSelectList();
    }
}
