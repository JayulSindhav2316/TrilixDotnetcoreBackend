using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Max.Data.Interfaces;
using Max.Data.Repositories;
using Max.Data.DataModel;
using Max.Core;
using System;

namespace Max.Data.Repositories
{
    public class AccessTokenRepository : Repository<Accesstoken>, IAccessTokenRepository
    {
        public AccessTokenRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<IEnumerable<Accesstoken>> GetAccesTokensByUserIdAsync(int userId)
        {
            var tokens = await membermaxContext.Accesstokens
                        .Where(x => x.UserId == userId)
                        .ToListAsync();

            return tokens;
        }

        public async Task<Accesstoken> GetAccesTokensByRequestAsync(int userId, string refreshToken, string ipAddress)
        {
            var token = await membermaxContext.Accesstokens
                       .Where(x => x.UserId == userId
                               && x.RefreshToken == refreshToken
                               && x.CreatedIp == ipAddress
                               && x.Expire > DateTime.UtcNow)
                       .FirstOrDefaultAsync();

            return token;
        }

        public async Task<IEnumerable<Accesstoken>> GetAccesTokensByEntityIdAsync(int entityId)
        {
            var tokens = await membermaxContext.Accesstokens
                        .Where(x => x.EntityId == entityId)
                        .ToListAsync();

            return tokens;
        }

        public async Task<Accesstoken> GetEntityAccesTokensByRequestAsync(int entityId, string refreshToken, string ipAddress)
        {
            var token = await membermaxContext.Accesstokens
                       .Where(x => x.EntityId == entityId
                               && x.RefreshToken == refreshToken
                               && x.CreatedIp == ipAddress
                               && x.Expire > DateTime.UtcNow)
                       .FirstOrDefaultAsync();

            return token;
        }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }

    }
}
