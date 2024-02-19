using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;

namespace Max.Data.Interfaces
{
    public interface IAccountingSetupRepository : IRepository<Accountingsetup>
    {

        Task<Accountingsetup> GetAccountingSetupByOrganizationIdAsync(int organizationId);

    }
}
