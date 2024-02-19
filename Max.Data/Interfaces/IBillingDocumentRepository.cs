using System;
using System.Collections.Generic;
using System.Text;
using Max.Data.DataModel;
using System.Threading.Tasks;

namespace Max.Data.Interfaces
{
    public interface IBillingDocumentRepository : IRepository<Billingdocument>
    {
        Task<IEnumerable<Billingdocument>> GetAllAutoBillingDocumentDetailsAsync();
        Task<int> GetLastBillingDocumentIdAsync();
        Task<int> GetCurrentBillingDocumentIdAsync();
    }
}
