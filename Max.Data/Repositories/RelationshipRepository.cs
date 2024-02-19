using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Max.Data.Interfaces;
using Max.Data.Repositories;
using Max.Data.DataModel;

namespace Max.Data.Repositories
{
    public class RelationshipRepository : Repository<Relationship>, IRelationshipRepository
    {
        public RelationshipRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<IEnumerable<Relationship>> GetAllRelationshipsAsync()
        {
            return await membermaxContext.Relationships
                .ToListAsync();
        }

        public async Task<Relationship> GetRelationshipByIdAsync(int id)
        {
            return await membermaxContext.Relationships
                .SingleOrDefaultAsync(m => m.RelationshipId == id);
        }

        public async Task<IEnumerable<Relation>> GetAllReverseRelationsByEntityIdAsync(int id)
        {
            return await membermaxContext.Relations
                .Include(x => x.Entity).Where(x => x.RelatedEntityId == id)
                .ToListAsync();
        }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }

    }
}
