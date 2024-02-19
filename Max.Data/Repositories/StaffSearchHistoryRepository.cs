using Max.Data.DataModel;
using Max.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Max.Data.Interfaces;
using Max.Data.Repositories;
using Max.Data.DataModel;
using Max.Core;
using System.Numerics;
using Max.Core.Helpers;

namespace Max.Data.Repositories
{
    public class StaffSearchHistoryRepository : Repository<Staffusersearchhistory>, IStaffSearchHistoryRepository
    {
        public StaffSearchHistoryRepository(membermaxContext context)
           : base(context)
        { }

        public async Task<bool> SaveSearchText(int userId, string text)
        {
            var entity = new Staffusersearchhistory();
            entity.StaffUserId = userId;
            entity.SearchText = text;
            entity.UpdatedAt = DateTime.Now;   
            var res = await membermaxContext.Staffusersearchhistories.AddAsync(entity).ConfigureAwait(false);
            return true;
        }

        public async Task<IEnumerable<Staffusersearchhistory>> GetSearchHistory(int userId)
        {
            var res = await membermaxContext.Staffusersearchhistories.Where(x => x.StaffUserId == userId).ToListAsync();  
            return res;
        }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }
    }
}
