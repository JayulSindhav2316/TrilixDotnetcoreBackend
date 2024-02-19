using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;

namespace Max.Data.Interfaces
{
    public interface IEmailRepository : IRepository<Email>
    {
        Task<IEnumerable<Email>> GetAllEmailsAsync();
        Task<Email> GetEmailByIdAsync(int id);
        Task<Email> GetPrimaryEmailByEmailAddressAsync(string email);
        Task<Email> GetPrimaryEmailByCompanyId(int companyId);
    }
}
