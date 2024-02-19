using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Max.Data.Interfaces;
using Max.Data.Repositories;
using Max.Data.DataModel;

namespace Max.Data.Repositories
{
    public class AddressRepository : Repository<Address>, IAddressRepository
    {
        public AddressRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<IEnumerable<Address>> GetAllAddressesAsync()
        {
            return await membermaxContext.Addresses
                .ToListAsync();
        }

        public async Task<Address> GetAddressByIdAsync(int id)
        {
            return await membermaxContext.Addresses
                .SingleOrDefaultAsync(m => m.AddressId == id);
        }
        public async Task<IEnumerable<Address>> GetAddressByPersonIdAsync(int id)
        {
            var addresses = await membermaxContext.Addresses
                                .Where(x => x.PersonId == id)
                                .ToListAsync();

            return addresses;
        }

        public async Task<IEnumerable<Address>> GetAddressByCompanyIdAsync(int id)
        {
            var addresses = await membermaxContext.Addresses
                                 .Where(x => x.CompanyId == id)
                                 .ToListAsync();
            return addresses;
        }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }

    }
}
