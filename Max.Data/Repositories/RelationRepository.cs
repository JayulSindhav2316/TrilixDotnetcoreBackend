using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Max.Data.Interfaces;
using Max.Data.Repositories;
using Max.Data.DataModel;
using Max.Core.Models;

namespace Max.Data.Repositories
{
    public class RelationRepository : Repository<Relation>, IRelationRepository
    {
        public RelationRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<IEnumerable<Relation>> GetAllRelationsAsync()
        {
            return await membermaxContext.Relations
                .ToListAsync();
        }

        public async Task<Relation> GetRelationByIdAsync(int id)
        {
            return await membermaxContext.Relations
                .SingleOrDefaultAsync(m => m.RelationId == id);
        }

        public async Task<IEnumerable<Relation>> GetAllRelationsByEntityIdAsync(int id)
        {
            return await membermaxContext.Relations
                .Where(x => x.EntityId == id)
                .Include(x => x.Relationship)
                .Include(x => x.RelatedEntity)
                .ToListAsync();
        }

        public async Task<IEnumerable<Relation>> GetAllReverseRelationsByEntityIdAsync(int id)
        {
            return await membermaxContext.Relations
                .Include(x => x.Relationship)
                .Include(x => x.Entity).Where(x => x.RelatedEntityId == id)
                .ToListAsync();
        }

        public async Task<IEnumerable<Relation>> GetRevereAndNonReverseRelationsByEntityIdAsync(int id)
        {
            return await membermaxContext.Relations
                .Include(x => x.Relationship)
                .Include(x => x.RelatedEntity).Where(x => x.EntityId == id || x.RelatedEntityId == id)
                .ToListAsync();
        }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }

    }
}
