using Max.Data.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Max.Data.Interfaces
{
    public interface IPipelineStageRepository : IRepository<Pipelinestage>
    {
        Task<IEnumerable<Pipelinestage>> GetAllPipelineStagesByPipelineIdAsync(int id);
        Task<IEnumerable<Pipelinestage>> GetAllActivePipelineStagesByPipelineIdAsync(int id);
        Task<Pipelinestage> GetPiplelineStageByIdAsync(int id);
    }
}
