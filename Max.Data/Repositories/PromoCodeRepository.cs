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
    public class PromoCodeRepository : Repository<Promocode>, IPromoCodeRepository
    {
        public PromoCodeRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<IEnumerable<Promocode>> GetAllPromoCodesAsync()
        {
            var promoCodes = await membermaxContext.Promocodes
                            .Include(x => x.GlAccount)
                            .ToListAsync();

            return promoCodes;
        }

        public async Task<IEnumerable<Promocode>> GetAllActivePromoCodesAsync()
        {
            var promoCodes = await membermaxContext.Promocodes
                            .Where(x => x.Status == (int)Status.Active)
                            .Include(x => x.GlAccount)
                            .ToListAsync();

            return promoCodes;
        }

        public async Task<Promocode> GetPromoCodeByIdAsync(int id)
        {
            var promoCode = await membermaxContext.Promocodes
                            .Where( x=> x.PromoCodeId == id)
                            .Include(x => x.GlAccount)
                                .FirstOrDefaultAsync();

            return promoCode;
        }

        public async Task<Promocode> GetPromoCodeByCodeAsync(string code)
        {
            var promoCode = await membermaxContext.Promocodes
                            .Where(x => x.Code == code)
                                .FirstOrDefaultAsync();

            return promoCode;
        }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }

    }
}
