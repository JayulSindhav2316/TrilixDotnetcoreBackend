using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Max.Data.Interfaces;
using Max.Data.Repositories;
using Max.Data.DataModel;
using Max.Core;

namespace Max.Data.Repositories
{
    public class OpportunityPipelineRepository : Repository<Opportunitypipeline>, IOpportunityPipelineRepository
    {
        public OpportunityPipelineRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<IEnumerable<Opportunitypipeline>> GetAllPipelinesAsync()
        {
            return await membermaxContext.Opportunitypipelines
                .Include(x => x.Pipelinestages)
                .Include(x => x.Pipelineproducts)
                .OrderBy(x => x.Name)
                .ToListAsync();
        }
        public async Task<IEnumerable<Opportunitypipeline>> GetAllActivePipelinesAsync()
        {
            return await membermaxContext.Opportunitypipelines
                .Where(x => x.Status == (int)Status.Active)
                .Include(x => x.Pipelinestages)
                .Include(x => x.Pipelineproducts)
                .OrderByDescending(x=>x.PipelineId)
                .ToListAsync();
        }
        public async Task<Opportunitypipeline> GetPiplelineByIdAsync(int id)
        {
            return await membermaxContext.Opportunitypipelines
                .Where(x => x.PipelineId==id)
                .Include(x => x.Pipelinestages)
                .Include(x => x.Pipelineproducts)
                .FirstOrDefaultAsync();
        }

        public async Task<Opportunitypipeline> GetPipelineByNameAsync(string name)
        {
            return await membermaxContext.Opportunitypipelines
               .Where(x => x.Name == name)
               .Include(x => x.Pipelinestages)
               .Include(x => x.Pipelineproducts)
               .FirstOrDefaultAsync();
        }
        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }

    }
}
