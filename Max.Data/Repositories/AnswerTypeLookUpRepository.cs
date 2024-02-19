using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Max.Data.DataModel;
using Max.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Max.Data.Repositories
{
    public class AnswerTypeLookUpRepository : Repository<Answertypelookup>, IAnswerTypeLookUpRepository
    {
        public AnswerTypeLookUpRepository(membermaxContext context)
           : base(context)
        { }

        public async Task<IEnumerable<Answertypelookup>> GetAnswertypelookup()
        {
            return await membermaxContext.Answertypelookups
                .ToListAsync();
        }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }
    }
}

