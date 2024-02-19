using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;
using Max.Core.Models;

namespace Max.Services.Interfaces
{
    public interface IRelationshipService
    {
        Task<IEnumerable<Relationship>> GetAllRelationships();
        Task<Relationship> GetRelationshipById(int id);
        Task<List<SelectListModel>> GetSelectList();

    }
}
