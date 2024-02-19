using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Core.Models;
using Max.Data.DataModel;

namespace Max.Data.Interfaces
{
    public interface IAccessTokenRepository : IRepository<Accesstoken>
    {

        Task<IEnumerable<Accesstoken>> GetAccesTokensByUserIdAsync(int userId);
        Task<Accesstoken> GetAccesTokensByRequestAsync(int userId, string refreshToken, string ipAddress);
        Task<Accesstoken> GetEntityAccesTokensByRequestAsync(int entityId, string refreshToken, string ipAddress);
        Task<IEnumerable<Accesstoken>> GetAccesTokensByEntityIdAsync(int entityId);
    }
}
