using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;

namespace Max.Data.Interfaces
{
    public interface IEntityRepository : IRepository<Entity>
    {
        Task<IEnumerable<Entity>> GetAllEntitiesAsync();
        Task<Entity> GetEntityByIdAsync(int id);
        Task<Entity> GetEntityByPersonIdAsync(int id);
        Task<Entity> GetEntityByCompanyIdAsync(int id);
        Task<IEnumerable<Entity>> GetEntitiesByNameAsync(string name);
        Task<Entity> GetEntityDetailsByIdAsync(int id);
        Task<Entity> GetMembershipDetailByEntityId(int id);
        Task<Entity> GetMembershipHistoryByEntityId(int id);
        Task<IEnumerable<Entity>> GetEntitiesByIdsAsync(int[] entityIds);
        Task<Entity> GetEntityByUserNameAsync(string userName);
        Task<Entity> GetEntityByWebLoginNameAsync(string userName);
        Entity GetEntityById(int id);
    }
}
