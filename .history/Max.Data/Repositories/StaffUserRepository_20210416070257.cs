using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Max.Core.Models;
using Max.Core.Repositories;
using Max.Data.DataModel;

namespace Max.Data.Repositories
{
    public class StaffUserRepository : Repository<StaffUser>, IStaffUserRepository
    {
        public StaffUserRepository(membermaxContext context) 
            : base(context)
        { }

        public async Task<IEnumerable<Staffuser>> GetAllStaffAsync()
        {
            return await membermaxContext.Staffusers
                .ToListAsync();
        }

        public async Task<Staffuser> GetStaffUserByIdAsync(int id)
        {
            return await membermaxContext.Staffusers
                .SingleOrDefaultAsync(m => m.UserId == id);;
        }

       
        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }
    }
}