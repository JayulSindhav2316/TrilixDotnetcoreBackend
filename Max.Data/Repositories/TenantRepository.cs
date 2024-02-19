using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Max.Data.Interfaces;
using Max.Data.Repositories;
using Max.Data.DataModel;

namespace Max.Data.Repositories
{
    public class TenantRepository : Repository<Tenant>, ITenantRepository
    {
        public TenantRepository(maxtenantContext context)
            : base(context)
        { }

        public async Task<Tenant> GetTenantByOrganizationNameAsync(string organizationName)
        {
            var tenant = await maxtenantContext.Tenants
                                .Where(x => x.OrganizationName == organizationName)
                                .FirstOrDefaultAsync();

            return tenant;
        }
        public async Task<Tenant> GetTenantByOrganizationIdAsync(int organizationId)
        {
            var tenant = await maxtenantContext.Tenants
                                .Where(x => x.OrganizationId == organizationId)
                                .FirstOrDefaultAsync();

            return tenant;
        }

        public async Task<Tenant> GetTenantByIdAsync(string id)
        {
            var tenant = await maxtenantContext.Tenants
                                .Where(x => x.TenantId == id)
                                .FirstOrDefaultAsync();

            return tenant;
        }

        private maxtenantContext maxtenantContext
        {
            get { return Context as maxtenantContext; }
        }

    }
}
