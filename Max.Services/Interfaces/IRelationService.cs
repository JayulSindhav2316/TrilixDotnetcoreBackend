using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;
using Max.Core.Models;

namespace Max.Services.Interfaces
{
    public interface IRelationService
    {
        Task<IEnumerable<Relation>> GetAllRelations();
        Task<Relation> GetRelationById(int id);
        Task<Relation> CreateRelation(RelationModel RelationModel);
        Task<IEnumerable<RelationModel>> GetRelationsByEntityId(int id);
        Task<IEnumerable<Relation>> GetReverseRelationsByEntityId(int id);
        Task<Relation> UpdateRelation(RelationModel model);
        Task<bool> DeleteRelation(int id);
        Task<List<SelectListModel>> GetRelationSelectListByEntityId(int entityId);
        Task<bool> AddOrUpdateRelation(List<RelationModel> RelationModel);
    }
}
