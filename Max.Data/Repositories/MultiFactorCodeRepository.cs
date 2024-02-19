using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Max.Data.Interfaces;
using Max.Data.Repositories;
using Max.Data.DataModel;

namespace Max.Data.Repositories
{
    public class MultiFactorCodeRepository : Repository<Multifactorcode>, IMultiFactorCodeRepository
    {
        public MultiFactorCodeRepository(membermaxContext context)
           : base(context)
        { }

        public async Task<Multifactorcode> GetByUserIdAsync(int userId)
        {
            return await membermaxContext.Multifactorcodes
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatDate)
                .FirstOrDefaultAsync();
        }

        public async Task<Multifactorcode> GetByUserNameAsync(string userName)
        {
            return await membermaxContext.Multifactorcodes
                .Include(x => x.User)
               .Where(x => x.User.UserName == userName)
               .FirstOrDefaultAsync();
        }

        public async Task<Multifactorcode> GetByAccessCodeAsync(string accessCode)
        {
            return await membermaxContext.Multifactorcodes
               .Where(x => x.AccessCode == accessCode)
               .FirstOrDefaultAsync();
        }

        public async Task<Multifactorcode> GetByOtpCodeAsync(string otpCode)
        {
            return await membermaxContext.Multifactorcodes
               .Where(x => x.Otpcode == otpCode)
               .FirstOrDefaultAsync();
        }

        public async Task<Multifactorcode> GetByEntityIdAsync(int entityId)
        {
            return await membermaxContext.Multifactorcodes
                .Include(x => x.Entity)
                .Where(x => x.EntityId == entityId)
                .OrderByDescending(x => x.CreatDate)
                .FirstOrDefaultAsync();
        }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }

    }
}
