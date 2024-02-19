using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Max.Data.Interfaces;
using Max.Data.Repositories;
using Max.Data.DataModel;

namespace Max.Data.Repositories
{
    public class TagRepository : Repository<Tag>, ITagRepository
    {
        public TagRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<IEnumerable<Tag>> GetAllTagsAsync()
        {
            return await membermaxContext.Tags
                .ToListAsync();
        }

        public async Task<Tag> GetTagByIdAsync(int id)
        {
            return await membermaxContext.Tags
                .SingleOrDefaultAsync(m => m.TagId == id);
        }

        public async Task<Tag> GetTagByNameAsync(string name)
        {
            return await membermaxContext.Tags
                .SingleOrDefaultAsync(m => m.TagName == name);
        }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }

    }
}
