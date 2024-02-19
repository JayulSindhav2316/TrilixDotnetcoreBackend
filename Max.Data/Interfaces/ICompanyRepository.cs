using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;

namespace Max.Data.Interfaces
{
    public interface ICompanyRepository : IRepository<Company>
    {
        Task<IEnumerable<Company>> GetAllCompaniesAsync();
        Task<Company> GetCompanyByIdAsync(int id);
        Task<IEnumerable<Company>> GetCompaniesByName(string name);
        Task<IEnumerable<Company>> GetCompanyByBillableContactId(int id);
        Task<IEnumerable<Company>> GetCompaniesByQuickSearchAsync(string searchParameter);
        Task<Company> GetCompanyByEmailIdAsync(string email);
        Task<Company> GetLastAddedCompanyAsync();

    }
}
