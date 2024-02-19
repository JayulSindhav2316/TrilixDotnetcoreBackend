using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Core.Models;
using Max.Data.DataModel;

namespace Max.Data.Interfaces
{
    public interface ICreditTransactionRepository : IRepository<Credittransaction>
    {

        Task<IEnumerable<Credittransaction>> GetCreditsByEntityIdAsync(int entityId);
        Task<decimal> GetCreditBalanceByEntityIdAsync(int entityId);
        Task<IEnumerable<Credittransaction>> GetCreditTransactionsByReceiptIdAsync(int receiptId);

    }
}
