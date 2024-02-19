using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Core.Models;
using Max.Data.DataModel;

namespace Max.Data.Interfaces
{
    public interface IWriteOffRepository : IRepository<Writeoff>
    {
        Task<IEnumerable<Writeoff>> GetWriteOffByInvoiceDetailIdAsync(int invoiceDetailId);
    }
}
