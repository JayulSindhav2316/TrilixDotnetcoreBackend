using Max.Core.Models;
using Max.Data.DataModel;
using Max.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Max.Core;
using Max.Core.Helpers;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Max.Data.Repositories
{
    public class ContactActivityInteractionRepository : Repository<Contactactivityinteraction>, IContactActivityInteractionRepository
    {
        public ContactActivityInteractionRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<Contactactivityinteraction> GetByAccountContactActivityIdAsync(int id, int? accountId, int? contactId)
        {
            return await membermaxContext.Contactactivityinteractions
                  .SingleOrDefaultAsync(x => x.ContactActivityId == id
                   && x.InteractionAccountId == accountId
                   && x.InteractionEntityId == contactId);
        }
        public async Task<Contactactivityinteraction> GetByAccountContactIdAsync(int? accountId, int? contactId)
        {
            return await membermaxContext.Contactactivityinteractions
                  .SingleOrDefaultAsync(x => x.InteractionAccountId == accountId
                                     && x.InteractionEntityId == contactId);
        }
        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }
    }
}
