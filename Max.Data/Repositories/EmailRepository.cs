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
    public class EmailRepository : Repository<Email>, IEmailRepository
    {
        public EmailRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<IEnumerable<Email>> GetAllEmailsAsync()
        {
            return await membermaxContext.Emails
                .ToListAsync();
        }

        public async Task<Email> GetEmailByIdAsync(int id)
        {
            return await membermaxContext.Emails
                .FirstOrDefaultAsync(m => m.EmailId == id);
        }

        public async Task<Email> GetPrimaryEmailByEmailAddressAsync(string email)
        {
            return await membermaxContext.Emails.AsNoTracking()
               .FirstOrDefaultAsync(m => m.EmailAddress == email && m.IsPrimary== (int)Status.Active);
        }

        public async Task<Email> GetPrimaryEmailByCompanyId(int companyId)
        {
            return await membermaxContext.Emails.AsNoTracking()
               .FirstOrDefaultAsync(m => m.CompanyId == companyId && m.IsPrimary == (int)Status.Active);
        }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }

    }
}
