using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Max.Data.Interfaces;
using Max.Data.Repositories;
using Max.Data.DataModel;

namespace Max.Data.Repositories
{
    public class DocumentObjectAccessRepository : Repository<Documentaccess>, IDocumentObjectAccessRepository
    {
        public DocumentObjectAccessRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<IEnumerable<Documentaccess>> GetAllDocumentAccessAsync()
        {
            return await membermaxContext.Documentaccesses
                                .ToListAsync();
        }
        public async Task<IEnumerable<Documentaccess>> GetDocumentAccessByDocumentObjectIdAsync(int id)
        {
            return await membermaxContext.Documentaccesses
                .Include(x => x.DocumentObject)
                .Include(x => x.Group)
                //.Include(x=>x.Roles)
                .Where(x => x.DocumentObjectId == id)
                .ToListAsync();
        }
        public async Task<Documentaccess> GetDocumentObjectAccessByIdAsync(int id)
        {
            return await membermaxContext.Documentaccesses
                .Include(x => x.DocumentObject)
                .Where(x => x.DocumentAccessId == id)
                .FirstOrDefaultAsync();
        }
        public async Task<IEnumerable<Documentaccess>> GetDocumentObjectAccessListByMembershipTypeIdAsync(int id)
        {
            return await membermaxContext.Documentaccesses
               .Include(x => x.DocumentObject)
               .Include(x => x.MembershipType)
               .Where(x => x.MembershipTypeId == id)
              .ToListAsync();
        }

        public async Task<IEnumerable<Documentaccess>> GetDocumentObjectAccessListByGroupIdAsync(int id)
        {
            return await membermaxContext.Documentaccesses
               .Include(x => x.DocumentObject)
               .Include(x => x.Group)
               .Where(x => x.GroupId == id)
              .ToListAsync();
        }
        public async Task<IEnumerable<Documentaccess>> GetDocumentObjectAccessListByStaffRoleIdAsync(int id)
        {
            return await membermaxContext.Documentaccesses
               .Include(x => x.DocumentObject)
               .Include(x => x.StaffRole)
               .Where(x => x.StaffRoleId == id)
              .ToListAsync();
        }
        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }

    }
}
