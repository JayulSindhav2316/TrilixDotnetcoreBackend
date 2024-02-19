using Max.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Max.Services.Interfaces
{
    public interface IOpportunityService
    {
        //Opportunity Pipelines

        Task<OpportunityPipelineModel> CreateOpportunityPipeline(OpportunityPipelineModel model);
        Task<bool> UpdateOpportunityPipeline(OpportunityPipelineModel model);
        Task<IEnumerable<OpportunityPipelineModel>> GetAllOpportunityPipelines();
        Task<OpportunityPipelineModel> GetOpportunityPipelineById(int id);
        Task<bool> DeleteOpportunityPipeline(int id);
        Task<OpportunityPipelineModel> CloneOpportunityPipeline(OpportunityPipelineModel model);
        Task<IEnumerable<OpportunityPipelineModel>> GetAllActiveOpportunityPipelines();
        Task<bool> UpdatePipelineStage(PipelineStageModel model);
        Task<bool> DeletePipelineStage(int id);
        Task<PipelineProductModel> CreatePipelineProduct(PipelineProductModel model);
        Task<bool> UpdatePipelineProduct(PipelineProductModel model);
        Task<bool> DeletePipelineProduct(int id);

        //Opportunity

        Task<OpportunityModel> CreateOpportunity(OpportunityModel model);
        Task<bool> UpdateOpportunity(OpportunityModel model);
        Task<IEnumerable<OpportunityModel>> GetAllOpportunites();
        Task<IEnumerable<OpportunityModel>> GetAllOpportunitesByAccountContactId(int id);
        Task<IEnumerable<OpportunityModel>> GetAllOpportunitesByPipelineId(int id);
        Task<IEnumerable<OpportunityModel>> GetAllOpportunitesByCompanyId(int id);
        Task<OpportunityModel> GetOpportunityById(int id);
        Task<IEnumerable<OpportunityStageGroupModel>> GetAllOpportunitesByPipelineIdGroupedByStages(int pipelineId);
        Task<IEnumerable<OpportunityStageGroupModel>> MoveOpportunityToAnotherStage(OpportunityModel model);
        Task<IEnumerable<OpportunityModel>> GetOpportunitesByStageId(int id);
        Task<IEnumerable<OpportunityModel>> GetOpportunitesByProductId(int id);
        Task<List<PipelineStageModel>> GetStagesByPipelineId(int pipelineId);
        Task<IEnumerable<OpportunityStageGroupModel>> GetOpportunitiesBySearchText(int pipelineId, string searchText);


        //SelectLists

        Task<List<SelectListModel>> GetPipelineSelectList();
        Task<List<SelectListModel>> GetStageSelectList(int pipelineId);
        Task<List<SelectListModel>> GetProductSelectList(int pipelineId);
    }
}