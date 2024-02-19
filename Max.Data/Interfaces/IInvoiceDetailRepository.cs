using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;

namespace Max.Data.Interfaces
{
    public interface IInvoiceDetailRepository : IRepository<Invoicedetail>
    {
        Task<IEnumerable<Invoicedetail>> GetAllInvoiceDetailsAsync();
        Task<Invoicedetail> GetInvoiceDetailByIdAsync(int id);
        decimal GetInvoiceItemBalanceById(int id);
        Task<IEnumerable<Invoicedetail>> GetAllInvoiceDetailsByInvoiceIdAsync(int  invoiceId);
        Task<IEnumerable<Invoicedetail>> GetInvoiceDetailsByReceiptId(int receiptId);
        Task<IEnumerable<Invoicedetail>> GetAllInvoiceDetailsByEntityIdAsync(int entityId);
        Task<IEnumerable<Invoicedetail>> GetInvoiceDetailsByBillableEntityIdAsync(int entityId);
    }
}
