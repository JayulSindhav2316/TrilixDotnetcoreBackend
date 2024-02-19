using Max.Data.DataModel;
using Max.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Max.Data.Repositories
{
    public class ConfigurationRepository : Repository<Configuration>, IConfigurationRepository
    {
        public ConfigurationRepository(membermaxContext context)
           : base(context)
        { }
        public async Task<Configuration> GetConfigurationByOrganizationIdAsync(int id)
        {
            return await membermaxContext.Configurations
               .Where(x => x.OrganizationId==id)
               .FirstOrDefaultAsync();
        }
        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }

    }
}
