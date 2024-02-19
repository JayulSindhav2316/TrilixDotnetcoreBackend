using Max.Data.DataModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Max.Data.Interfaces
{
    public interface ITenantRepository : IRepository<Tenant>
    {

        Task<Tenant> GetTenantByOrganizationNameAsync(string organizationName);
        Task<Tenant> GetTenantByOrganizationIdAsync(int organizationId);
        Task<Tenant> GetTenantByIdAsync(string tenantId);

    }
}
