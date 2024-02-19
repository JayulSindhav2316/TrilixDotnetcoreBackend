using Max.Core;
using Max.Data.DataModel;
using Max.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Max.Data.Repositories
{
    public class ContactTokenRepository : Repository<Contacttoken>, IContactTokenRepository
    {
        public ContactTokenRepository(membermaxContext context)
           : base(context)
        { }

        public async Task<Contacttoken> GetTokenRequestByIdAsync(int id)
        {
            return await membermaxContext.Contacttokens
                .SingleOrDefaultAsync(m => m.ContactTokenId == id);
        }
        public async Task<Contacttoken> GetTokenRequestByTokenAsync(string token)
        {
            return await membermaxContext.Contacttokens
                .SingleOrDefaultAsync(m => m.Token == token && m.Status == (int)Status.Active);
        }
        public async Task<Contacttoken> GetTokenRequestByEmailAsync(string emailAddress)
        {
            return await membermaxContext.Contacttokens
                .SingleOrDefaultAsync(m => m.Email == emailAddress);
        }
        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }
    }
}
