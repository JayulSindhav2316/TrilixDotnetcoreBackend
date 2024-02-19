using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;

namespace Max.Data.Interfaces
{
    public interface IBillingEmailRepository : IRepository<Billingemail>
    {
        Task<IEnumerable<Billingemail>> GetAllBillingEmailsAsync();
        Task<Billingemail> GetBillingEmailByIdAsync(int id);
        Task<Billingemail> GetBillingEmailByInvoiceIdAsync(int id);
        Task<IEnumerable<Billingemail>> GetBillingEmailsByCycleIdAsync(int cycleId);
        Task<Billingemail> GetBillingEmailsByCycleIdAndInvoiceIdAsync(int cycleId, int invoiceId);
        Task<Billingemail> GetBillingEmailByTokenAsync(string token);
    }
}
