using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;


namespace Max.Data.Interfaces
{
    public interface IMembershipCategoryRepsoitory : IRepository<Membershipcategory>
    {
        Task<IEnumerable<Membershipcategory>> GetAllMembershipCategoriesAsync();
        Task<Membershipcategory> GetMembershipCategoryByIdAsync(int id);
    }
}
