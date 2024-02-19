using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;

namespace Max.Data.Interfaces
{
    public interface IRelationRepository : IRepository<Relation>
    {
        Task<IEnumerable<Relation>> GetAllRelationsAsync();
        Task<Relation> GetRelationByIdAsync(int id);
        Task<IEnumerable<Relation>> GetAllRelationsByEntityIdAsync(int id);
        Task<IEnumerable<Relation>> GetAllReverseRelationsByEntityIdAsync(int id);
        Task<IEnumerable<Relation>> GetRevereAndNonReverseRelationsByEntityIdAsync(int id);
    }
}
