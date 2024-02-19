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
    public class ResetPasswordRepository : Repository<Resetpassword>, IResetPasswordRepository
    {
        public ResetPasswordRepository(membermaxContext context)
           : base(context)
        { }

        public async Task<Resetpassword> GetResetRequestByIdAsync(int id)
        {
            return await membermaxContext.Resetpasswords
                .SingleOrDefaultAsync(m => m.Id == id);
        }
        public async Task<Resetpassword> GetResetRequestByTokenAsync(string token)
        {
            return await membermaxContext.Resetpasswords
                .SingleOrDefaultAsync(m => m.Token == token && m.Status==(int)Status.Active);
        }
        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }
    }
}
