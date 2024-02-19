using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Max.Data.Interfaces;
using Max.Data.Repositories;
using Max.Data.DataModel;

namespace Max.Data.Repositories
{
    public class NoteRepository : Repository<Note>, INoteRepository
    {
        public NoteRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<IEnumerable<Note>> GetAllNotesAsync()
        {
            return await membermaxContext.Notes
                .OrderByDescending(x => x.CreatedOn)
                .ToListAsync();
        }

        public async Task<Note> GetNoteByIdAsync(int id)
        {
            return await membermaxContext.Notes
                .SingleOrDefaultAsync(m => m.NoteId == id);
        }

        public async Task<IEnumerable<Note>> GetNotesByEntityIdAsync(int entityId)
        {
            return await membermaxContext.Notes
                .Where(x => x.EntityId == entityId)
                .OrderByDescending(x => x.CreatedOn)
               .ToListAsync();
        }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }

    }
}
