using Max.Data.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Max.Data.Interfaces
{
    public interface IOpportunityPipelineRepository : IRepository<Opportunitypipeline>
    {
        Task<IEnumerable<Opportunitypipeline>> GetAllPipelinesAsync();
        Task<IEnumerable<Opportunitypipeline>> GetAllActivePipelinesAsync();
        Task<Opportunitypipeline> GetPiplelineByIdAsync(int id);
        Task<Opportunitypipeline> GetPipelineByNameAsync(string name);
    }
}
