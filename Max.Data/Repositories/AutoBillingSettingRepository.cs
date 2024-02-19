using System;
using System.Collections.Generic;
using System.Text;
using Max.Data.DataModel;
using Max.Data.Interfaces;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Max.Data.Repositories
{
    public class AutoBillingSettingRepository : Repository<Autobillingsetting>, IAutoBillingSettingRepository
    {
        public AutoBillingSettingRepository(membermaxContext context)
            : base(context)
        { }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }
    }
}
