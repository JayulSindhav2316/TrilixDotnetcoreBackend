using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Max.Data.Interfaces;
using Max.Data.Repositories;
using Max.Data.DataModel;
using Max.Core.Models;

namespace Max.Data.Repositories
{
    public class AccessLogRepository : Repository<Accesslog>, IAccessLogRepository
    {
        public AccessLogRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<IEnumerable<Accesslog>> GetAccessLogByUserNameAsync(string userName)
        {
            var addresses = await membermaxContext.Accesslogs
                                .Where(x => x.UserName == userName)
                                .ToListAsync();

            return addresses;
        }
        public async Task<IEnumerable<LoginStatistics>> GetTop10AccessLogAsync()
        {
            var logins = await membermaxContext.Accesslogs
                                .Where(x => x.Portal == 1)
                                .GroupBy(x => new { x.UserName }) 
                                .Select(g => new LoginStatistics
                                {
                                    UserName = g.Key.UserName,
                                    Count = g.Count()
                                })
                                .OrderByDescending(x =>x.Count)
                                .Take(10)
                                .ToListAsync();

            return logins;
        }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }

    }
}
