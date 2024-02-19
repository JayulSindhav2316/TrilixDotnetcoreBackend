using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Max.Data.DataModel;

namespace Max.Data.Interfaces
{
    public interface IInvoiceRepository : IRepository<Invoice>
    {
        Task<IEnumerable<Invoice>> GetAllInvoicesAsync();
        Task<Invoice> GetInvoiceByIdAsync(int id);
        Task<Invoice> GetInvoiceSummaryByIdAsync(int id);
        Task<Invoice> GetInvoicePaymentsByIdAsync(int id);
        Task<IEnumerable<Invoice>> GetAllInvoicesByEntityIdAsync(int entityId);
        Task<IEnumerable<Invoice>> GetAllInvoicesByEventIdAsync(int eventId);
        Task<IEnumerable<Invoice>> GetInvoicesBySearchConditionAsync(int personId, string serachBy, string item, DateTime start, DateTime end);
        Task<Invoice> GetInvoicePrintDetailsByIdAsync(int invocieId);
        Task<IEnumerable<Invoice>> GetMembershipInvoicePayments();
        Task<IEnumerable<Invoice>> GetAllActiveInvoicesWithDuesAsync();
        Task<IEnumerable<Invoice>> GetInvoicesByReceiptId(int id);
        decimal GetInvoiceBalanceByEntityId(int entityId);
        Task<Invoice> GetFinalizedInvoicesByMembershipId(int membershipId, DateTime nextBilldate);
        Task<IEnumerable<Invoice>> GetInvoicesByMultipleInvoiceIdsAsync(int[] invoiceIds);
        Task<IEnumerable<Invoice>> GetInvoicesWithBalanceByEntityIdAsync(int entityId);
    }
}
