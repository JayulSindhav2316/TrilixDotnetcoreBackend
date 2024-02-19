using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;
using Max.Core.Models;

namespace Max.Services.Interfaces
{
    public interface IInvoiceDetailService
    {
        Task<IEnumerable<Invoicedetail>> GetAllInvoiceDetails();
        Task<Invoicedetail> GetInvoiceDetailById(int id);
        Task<Invoicedetail> CreateInvoiceDetail(InvoiceDetailModel InvoiceDetailModel);
        Task<IEnumerable<Invoicedetail>> GetAllInvoiceDetailsByInvoiceId(int invoicecId);
    }
}
