using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Max.Data.Interfaces;
using Max.Data.Repositories;
using Max.Data.DataModel;

namespace Max.Data.Repositories
{
    public class DocumentContainerRepository : Repository<Documentcontainer>, IDocumentContainerRepository
    {
        public DocumentContainerRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<IEnumerable<Documentcontainer>> GetAllDocumentContainersAsync()
        {
            return await membermaxContext.Documentcontainers
                    .Include(x => x.Containeraccesses)
                        .ThenInclude(x => x.MembershipType)
                         .ThenInclude(x => x.CategoryNavigation)
                    .Include(x => x.Containeraccesses)
                        .ThenInclude(x => x.MembershipType)
                         .ThenInclude(x => x.PeriodNavigation)
                     .Include(x => x.Containeraccesses)
                        .ThenInclude(x => x.Group)
                         .Include(x => x.Containeraccesses)
                        .ThenInclude(x => x.StaffRole)
                    .ToListAsync();

        }
        public async Task<IEnumerable<Documentcontainer>> GetAllDocumentContainersWithObjectsAsync(int? entityId)
        {
                return await membermaxContext.Documentcontainers
                        .Include(x => x.Documentobjects)
                        .ToListAsync();
        }
        public async Task<Documentcontainer> GetDocumentContainerByIdAsync(int id)
        {
            return await membermaxContext.Documentcontainers
                      .Include(x => x.Containeraccesses)
                        .ThenInclude(x => x.MembershipType)
                        .ThenInclude(x => x.CategoryNavigation)
                        .Include(x => x.Containeraccesses)
                        .ThenInclude(x => x.Group)
                      .Include(x => x.Containeraccesses)
                      .ThenInclude(x => x.StaffRole)
                      .Where(x => x.ContainerId == id)
                      .FirstOrDefaultAsync();

        }
        public async Task<Documentcontainer> GetDocumentContainerByNameAsync(string name, int id)
        {
            return await membermaxContext.Documentcontainers
                       .Where(x => x.Name == name && x.ContainerId != id)
                       .FirstOrDefaultAsync();
        }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }

    }
}
