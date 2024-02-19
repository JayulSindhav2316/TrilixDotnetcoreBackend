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
    public class PipelineProductRepository : Repository<Pipelineproduct>, IPipelineProductRepository
    {
        public PipelineProductRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<IEnumerable<Pipelineproduct>> GetAllPipelineProductsByPipelineIdAsync(int id)
        {
            return await membermaxContext.Pipelineproducts
                .Where(p => p.PipelineId == id)
                .ToListAsync();
        }
        public async Task<IEnumerable<Pipelineproduct>> GetAllActivePipelineProductsByPipelineIdAsync(int id)
        {
            return await membermaxContext.Pipelineproducts
                .Where(x => x.Status == (int)Status.Active && x.PipelineId == id)
                .ToListAsync();
        }
        public async Task<Pipelineproduct> GetPiplelineProductByIdAsync(int id)
        {
            return await membermaxContext.Pipelineproducts
                .Where(x => x.ProductId == id)
                .FirstOrDefaultAsync();
        }
        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }

    }
}
