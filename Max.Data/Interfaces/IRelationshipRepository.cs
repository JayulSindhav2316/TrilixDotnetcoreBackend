using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Core.Models;
using Max.Data.DataModel;

namespace Max.Data.Interfaces
{
    public interface IRelationshipRepository : IRepository<Relationship>
    {
        Task<IEnumerable<Relationship>> GetAllRelationshipsAsync();
        Task<Relationship> GetRelationshipByIdAsync(int id);
        Task<IEnumerable<Relation>> GetAllReverseRelationsByEntityIdAsync(int id);

    }
}
