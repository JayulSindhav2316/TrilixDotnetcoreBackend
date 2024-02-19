
using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;

namespace Max.Data.Interfaces
{
    public interface IMembershipTypeRepository : IRepository<Membershiptype>
    {
        Task<IEnumerable<Membershiptype>> GetAllMembershipTypesAsync();
        Task<Membershiptype> GetMembershipTypeByIdAsync(int id);
        Task<Membershiptype> GetMembershipTypeByNameAndCategoryAsync(string name, int category);
        Task<Membershiptype> GetMembershipTypeByNameAndCategoryAndFrequencyAsync(string name, int category, int frequency);
        Task<IEnumerable<Membershiptype>> GetMembershipTypesByCategoriesAsync(int[] id);
    }

}
