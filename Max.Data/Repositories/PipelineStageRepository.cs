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
    public class PipelineStageRepository : Repository<Pipelinestage>, IPipelineStageRepository
    {
        public PipelineStageRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<IEnumerable<Pipelinestage>> GetAllPipelineStagesByPipelineIdAsync(int id)
        {
            return await membermaxContext.Pipelinestages
                .Where(p => p.PipelineId == id)
                .ToListAsync();
        }
        public async Task<IEnumerable<Pipelinestage>> GetAllActivePipelineStagesByPipelineIdAsync(int id)
        {
            return await membermaxContext.Pipelinestages
                .Where(x => x.Status == (int)Status.Active && x.PipelineId == id)
                .ToListAsync();
        }
        public async Task<Pipelinestage> GetPiplelineStageByIdAsync(int id)
        {
            return await membermaxContext.Pipelinestages
                .Where(x => x.StageId == id)
                .FirstOrDefaultAsync();
        }
        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }

    }
}
