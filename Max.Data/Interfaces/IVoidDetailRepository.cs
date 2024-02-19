using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Core.Models;
using Max.Data.DataModel;

namespace Max.Data.Interfaces
{
    public interface IVoidDetailRepository : IRepository<Voiddetail>
    {

        //Task<IEnumerable<Refunddetail>> GetRefundsByEntityIdAsync(int id);
        Task<IEnumerable<Voiddetail>> GetVoidByReceiptIdAsync(int receiptId);
    }
}
