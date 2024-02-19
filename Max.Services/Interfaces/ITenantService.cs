
using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;
using Max.Core.Models;

namespace Max.Services.Interfaces
{
    public interface ITenantService
    {
        Task<Tenant> GetTenantByOrganizationName(string organizationName);
        Task<Tenant> GetTenantByOrganizationId(int organizationId);
        Task<Tenant> GetTenantById(string tenantId);
        Task<IEnumerable<Tenant>> GetAllTenants();
    }
}