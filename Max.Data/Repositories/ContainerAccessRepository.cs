using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Max.Data.Interfaces;
using Max.Data.Repositories;
using Max.Data.DataModel;

namespace Max.Data.Repositories
{
    public class ContainerAccessRepository : Repository<Containeraccess>, IContainerAccessRepository
    {
        public ContainerAccessRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<IEnumerable<Containeraccess>> GetAllContainerAccessAsync()
        {
            return await membermaxContext.Containeraccesses
                                .ToListAsync();
        }
        public async  Task<IEnumerable<Containeraccess>> GetContainerAccessByContainerIdAsync(int id)
        {
            return await membermaxContext.Containeraccesses
                .Include(x => x.Container)
                .Where(x => x.ContainerId == id)
                .ToListAsync();
        }
        public async  Task<Containeraccess> GetContainerAccessByIdAsync(int id)
        {
            return  await membermaxContext.Containeraccesses
                .Include(x => x.Container)
                .Where(x => x.ContainerAccessId == id)
                .FirstOrDefaultAsync();
        }
        public async Task<IEnumerable<Containeraccess>> GetContainerAccessByMembershipTypeIdAsync(int id)
        {
            return await membermaxContext.Containeraccesses
               .Include(x => x.Container)
               .Include(x => x.MembershipType)
               .Where(x => x.MembershipTypeId == id)
              .ToListAsync();
        }

        public async Task<IEnumerable<Containeraccess>> GetContainerAccessByGroupIdAsync(int id)
        {
            return await membermaxContext.Containeraccesses
               .Include(x => x.Container)
               .Include(x => x.Group)
               .Where(x => x.GroupId == id)
              .ToListAsync();
        }
        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }

    }
}
