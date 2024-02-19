using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Max.Data.Interfaces;
using Max.Data.Repositories;
using Max.Data.DataModel;

namespace Max.Data.Repositories
{
    public class MembershipTypeRepository : Repository<Membershiptype>, IMembershipTypeRepository
    {
        public MembershipTypeRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<IEnumerable<Membershiptype>> GetAllMembershipTypesAsync()
        {
            return await membermaxContext.Membershiptypes
                .Include(x => x.PeriodNavigation)
                .Include(i => i.Membershipfees)
                    .ThenInclude(x => x.GlAccount)
                .Include(n => n.PeriodNavigation)
                .Include(x => x.CategoryNavigation)
                .ToListAsync();
        }

        public async Task<Membershiptype> GetMembershipTypeByIdAsync(int id)
        {
            return await membermaxContext.Membershiptypes
                .Include(x => x.PeriodNavigation)
                .Include(x => x.Membershipfees)
                .SingleOrDefaultAsync(m => m.MembershipTypeId == id);
        }

        public async Task<Membershiptype> GetMembershipTypeByNameAndCategoryAsync(string name, int category)
        {
            return await membermaxContext.Membershiptypes
                .SingleOrDefaultAsync(m => m.Name == name && m.Category == category);
        }

        public async Task<Membershiptype> GetMembershipTypeByNameAndCategoryAndFrequencyAsync(string name, int category, int frequency)
        {
            return await membermaxContext.Membershiptypes
                .SingleOrDefaultAsync(m => m.Name == name && m.Category == category && m.PaymentFrequency == frequency);
        }

        public async Task<IEnumerable<Membershiptype>> GetMembershipTypesByCategoriesAsync(int[] categoryIds)
        {
            return await membermaxContext.Membershiptypes
                .Where(x => categoryIds.Contains(x.CategoryNavigation.MembershipCategoryId))
                .Include(x => x.CategoryNavigation)
                .Include(x => x.PeriodNavigation)
                .Include(x => x.Membershipfees)
                .ToListAsync();

        }
        

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }
    }
}