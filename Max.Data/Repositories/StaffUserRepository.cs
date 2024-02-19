using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Max.Data.Interfaces;
using Max.Data.Repositories;
using Max.Data.DataModel;
using Max.Core.Helpers;
using Max.Core;

namespace Max.Data.Repositories
{
    public class StaffUserRepository : Repository<Staffuser>, IStaffUserRepository
    {
        public StaffUserRepository(membermaxContext context) 
            : base(context)
        { }

        public async Task<IEnumerable<Staffuser>> GetAllStaffUsersAsync()
        {
            return await membermaxContext.Staffusers
                .Include(x=>x.Department)
                .Include(i => i.Staffroles)
                .ThenInclude(r =>r.Role)
                .ToListAsync();
        }

        public async Task<Staffuser> GetStaffUserByIdAsync(int id)
        {
            return await membermaxContext.Staffusers
                .Include(i => i.Staffroles)
                    .ThenInclude(r => r.Role)
                .SingleOrDefaultAsync(m => m.UserId == id);
        }

        public Staffuser GetStaffUserById(int id)
        {
            return membermaxContext.Staffusers
                .Include( x => x.Organization)
                .AsNoTracking()
                .SingleOrDefault(m => m.UserId == id);
        }

        public async Task<Staffuser> GetStaffUserByUserNameAsync(string userName)
        {
            return await membermaxContext.Staffusers
                .SingleOrDefaultAsync(m => m.UserName == userName); 
        }

        public async Task<Staffuser> GetStaffUserByEmailAsync(string email)
        {
            return await membermaxContext.Staffusers
                .Include(x => x.Organization)
                .SingleOrDefaultAsync(m => m.Email == email);
        }

        public async Task<IEnumerable<Staffuser>> GetStaffByFirstORLastNameAsync(string value)
        {
            return await membermaxContext.Staffusers
                .Where(x => x.FirstName.StartsWith(value) || x.LastName.StartsWith(value) || x.Email.StartsWith(value) || x.UserName.StartsWith(value))
                .Where(x => x.Status == 1)
                .OrderBy(x => x.LastName).ThenBy(x => x.FirstName)
                .AsNoTracking()
                .Take(100)
                .ToListAsync();
        }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }
    }
}