using Max.Data.DataModel;
using Max.Services.Interfaces;
using Max.Core.Models;
using Max.Core;
using Max.Data;
using Max.Services;
using System.Linq;
using Max.Data.Interfaces;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Max.Services
{
    public class RelationshipService : IRelationshipService
    {

        private readonly IUnitOfWork _unitOfWork;
        public RelationshipService(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Relationship>> GetAllRelationships()
        {
            return await _unitOfWork.Relationships
                .GetAllRelationshipsAsync();
        }

        public async Task<Relationship> GetRelationshipById(int id)
        {
            return await _unitOfWork.Relationships
                .GetRelationshipByIdAsync(id);
        }

        public async Task<List<SelectListModel>> GetSelectList()
        {
            var relationships = await _unitOfWork.Relationships.GetAllRelationshipsAsync();

            List<SelectListModel> selectList = new List<SelectListModel>();
            foreach (var relationship in relationships)
            {
                if(!selectList.Where(x => x.name == relationship.Relation).Any())
                {
                    SelectListModel selectListItem = new SelectListModel();
                    selectListItem.code = relationship.RelationshipId.ToString();
                    selectListItem.name = relationship.Relation;
                    selectList.Add(selectListItem);
                }
               
            }

            return selectList;
        }
        
    }
}