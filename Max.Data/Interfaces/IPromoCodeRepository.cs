using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;

namespace Max.Data.Interfaces
{
    public interface IPromoCodeRepository : IRepository<Promocode>
    {

        Task<IEnumerable<Promocode>> GetAllPromoCodesAsync();
        Task<IEnumerable<Promocode>> GetAllActivePromoCodesAsync();
        Task<Promocode> GetPromoCodeByIdAsync(int id);
        Task<Promocode> GetPromoCodeByCodeAsync(string code);
        

    }
}
