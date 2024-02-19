using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;
using Max.Core.Models;

namespace Max.Services.Interfaces
{
    public interface IPromoCodeService
    {
        Task<IEnumerable<Promocode>> GetAllPromoCodes();
        Task<Promocode> GetPromoCodeById(int id);
        Task<PromoCodeModel> GetPromoCodeByCode(string code);
        Task<Promocode> CreatePromoCode(PromoCodeModel model);
        Task<bool> UpdatePromoCode(PromoCodeModel model);
        Task<bool> DeletePromoCode(int promoCodeId);
        Task<PromoCodeModel> GenratePromoCode();

    }
}
