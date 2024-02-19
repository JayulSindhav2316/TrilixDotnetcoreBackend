using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Max.Data.Interfaces;
using Max.Data.DataModel;


namespace Max.Data.Repositories
{
    public class MembershipCategoryRepository : Repository<Membershipcategory>, IMembershipCategoryRepsoitory
    {
        public MembershipCategoryRepository(membermaxContext context)
           : base(context)
        { }

        public async Task<IEnumerable<Membershipcategory>> GetAllMembershipCategoriesAsync()
        {
            return await membermaxContext.Membershipcategories
                .ToListAsync();
        }

        public async Task<Membershipcategory> GetMembershipCategoryByIdAsync(int id)
        {
            return await membermaxContext.Membershipcategories
                .SingleOrDefaultAsync(m => m.MembershipCategoryId == id);
        }
        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }

    }
}
