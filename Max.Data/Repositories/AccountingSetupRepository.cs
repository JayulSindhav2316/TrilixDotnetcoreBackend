using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Max.Data.Interfaces;
using Max.Data.Repositories;
using Max.Data.DataModel;
using Max.Core;
using System;

namespace Max.Data.Repositories
{
    public class AccountingSetupRepository : Repository<Accountingsetup>, IAccountingSetupRepository
    {
        public AccountingSetupRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<Accountingsetup> GetAccountingSetupByOrganizationIdAsync(int organizationId)
        {
            var setup = await membermaxContext.Accountingsetups                
                .Where(x => x.OrganizationId == organizationId)
                .FirstOrDefaultAsync();

            return setup;
        }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }

    }
}
