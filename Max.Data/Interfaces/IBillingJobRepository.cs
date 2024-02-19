using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;

namespace Max.Data.Interfaces
{
    public interface IBillingJobRepository : IRepository<Billingjob>
    {
        Task<IEnumerable<Billingjob>> GetAllBillingJobsAsync();
        Task<Billingjob> GetBillingJobByIdAsync(int id);
        Task<Billingjob> GetBillingJobByCycleIdAsync(int id);
        Task<Billingjob> GetBillingJobByDateAsync(DateTime date);
        Task<Billingjob> GetBillingFinalizationJobByDateAsync(DateTime date);
        Task<Billingjob> GetNextBillingJobAsync();
        Task<Billingjob> GetNextRenewalJobAsync();
        Task<Billingjob> GetRenewalFinalizationJobByDateAsync(DateTime date);
    }
}
