using Max.Data.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Max.Data.Interfaces
{
    public interface IPipelineProductRepository : IRepository<Pipelineproduct>
    {
        Task<IEnumerable<Pipelineproduct>> GetAllPipelineProductsByPipelineIdAsync(int id);
        Task<IEnumerable<Pipelineproduct>> GetAllActivePipelineProductsByPipelineIdAsync(int id);
        Task<Pipelineproduct> GetPiplelineProductByIdAsync(int id);
    }
}
