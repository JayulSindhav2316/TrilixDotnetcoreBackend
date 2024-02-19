using Max.Data.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Max.Data
{
    public static class TenantManager
    {
        public static string OrganizationName;

        public static List<Tenant> Tenants;

        public static Tenant GetTenantByOrganizationName(string organizationName)
        {
            return Tenants.Where(x => x.OrganizationName == organizationName).FirstOrDefault();
        }
    }

}
