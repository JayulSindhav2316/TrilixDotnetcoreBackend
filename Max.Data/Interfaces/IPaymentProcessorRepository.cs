
using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;

namespace Max.Data.Interfaces
{
    public interface IPaymentProcessorRepository : IRepository<Paymentprocessor>
    {
        Task<IEnumerable<Paymentprocessor>> GetAllPaymentProcessorsAsync();
        Task<Paymentprocessor> GetPaymentProcessorByIdAsync(int id);
        Task<Paymentprocessor> GetPaymentProcessorByOrganizationIdAsync(int id);

    }

}
