using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Max.Data.Interfaces;
using Max.Data.Repositories;
using Max.Data.DataModel;

namespace Max.Data.Repositories
{
    public class PhoneRepository : Repository<Phone>, IPhoneRepository
    {
        public PhoneRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<IEnumerable<Phone>> GetAllPhonesAsync()
        {
            return await membermaxContext.Phones
                .ToListAsync();
        }

        public async Task<Phone> GetPhoneByIdAsync(int id)
        {
            return await membermaxContext.Phones
                .SingleOrDefaultAsync(m => m.PhoneId == id);
        }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }

    }
}
