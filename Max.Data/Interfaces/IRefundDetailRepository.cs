using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Core.Models;
using Max.Data.DataModel;

namespace Max.Data.Interfaces
{
    public interface IRefundDetailRepository : IRepository<Refunddetail>
    {

        Task<IEnumerable<Refunddetail>> GetRefundsByEntityIdAsync(int id);
        Task<IEnumerable<Refunddetail>> GetRefundsByReceiptIdAsync(int receiptId);
    }
}
