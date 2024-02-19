using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Max.Data.Interfaces;
using Max.Data.Repositories;
using Max.Data.DataModel;
using Max.Core.Helpers;
using Max.Core;
using System;
using Max.Core.Models;
using System.Net.NetworkInformation;

namespace Max.Data.Repositories
{
    public class MembershipReportRepository : Repository<Membershipreport>, IMembershipReportRepository
    {
        public MembershipReportRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<IEnumerable<Membershipreport>> GetAllMembershipReportsAsync()
        {
            return await membermaxContext.Membershipreports
                .ToListAsync();
        }
        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }
    }
}