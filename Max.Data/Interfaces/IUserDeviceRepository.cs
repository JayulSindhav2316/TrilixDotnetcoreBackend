using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;

namespace Max.Data.Interfaces
{
    public interface IUserDeviceRepository : IRepository<Userdevice>
    {
        Task<IEnumerable<Userdevice>> GetUserDevicesByUserIdAsync(int userId);
        Task<IEnumerable<Userdevice>> GetUserDevicesByEntityIdAsync(int entityId);
    }
}
