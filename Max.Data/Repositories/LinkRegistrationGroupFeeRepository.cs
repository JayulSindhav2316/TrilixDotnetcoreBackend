using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Max.Data.DataModel;
using Max.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Max.Data.Repositories
{
    public class LinkRegistrationGroupFeeRepository : Repository<Linkregistrationgroupfee>, ILinkRegistrationGroupFeeRepository
    {
        public LinkRegistrationGroupFeeRepository(membermaxContext context)
          : base(context)
        { }

        public async Task<IEnumerable<Linkregistrationgroupfee>> GetLinkRegistrationGroupFeesByLinkEventGroupIdAsync(int linkEventgroupId)
        {
            return await membermaxContext.Linkregistrationgroupfees
               .Where(x => x.LinkEventGroupId == linkEventgroupId)
               .ToListAsync();
        }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }
    }
}
