using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Max.Data.Interfaces;
using Max.Data.Repositories;
using Max.Data.DataModel;

namespace Max.Data.Repositories
{
    public class UserDeviceRepository : Repository<Userdevice>, IUserDeviceRepository
    {
        public UserDeviceRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<IEnumerable<Userdevice>> GetUserDevicesByUserIdAsync(int userId)
        {
            var addresses = await membermaxContext.Userdevices
                                .Where(x => x.UserId == userId)
                                .ToListAsync();

            return addresses;
        }

        public async Task<IEnumerable<Userdevice>> GetUserDevicesByEntityIdAsync(int entityId)
        {
            var addresses = await membermaxContext.Userdevices
                                .Where(x => x.EntityId == entityId)
                                .ToListAsync();

            return addresses;
        }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }

    }
}
