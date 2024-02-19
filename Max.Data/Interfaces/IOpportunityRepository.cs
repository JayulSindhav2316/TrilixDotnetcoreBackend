using Max.Data.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Max.Data.Interfaces
{
    public interface IOpportunityRepository : IRepository<Opportunity>
    {
        Task<IEnumerable<Opportunity>> GetAllOpportunitiesAsync();
        Task<Opportunity> GetOpportunityByIdAsync(int id);
        Task<IEnumerable<Opportunity>> GetOpportunitiesByAccountContactAsync(int id);
        Task<IEnumerable<Opportunity>> GetOpportunitiesByPipelineIdAsync(int id);
        Task<IEnumerable<Opportunity>> GetOpportunitiesByStaffUserIdAsync(int id);
        Task<IEnumerable<Opportunity>> GetOpportunitiesByCompanyIdAsync(int id);
        Task<IEnumerable<Opportunity>> GetOpportunitiesByStageIdAsync(int id);
        Task<IEnumerable<Opportunity>> GetOpportunitiesByProductIdAsync(int id);
        Task<IEnumerable<Opportunity>> GetOpportunitiesBySearchTextAsync(int pipelineId, string searchText);
    }
}