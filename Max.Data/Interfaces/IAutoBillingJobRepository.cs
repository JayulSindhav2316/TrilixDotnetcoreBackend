using System;
using System.Collections.Generic;
using System.Text;
using Max.Data.DataModel;
using System.Threading.Tasks;

namespace Max.Data.Interfaces
{
   
        public interface IAutoBillingJobRepository : IRepository<Autobillingjob>
        {
            Task<Autobillingjob> GetAutoBillingJobByIdAsync(int id);
            Task<IEnumerable<Autobillingjob>> GetAllAutoBillingJobsAsync();
            Task<Autobillingjob> GetAutoBillingJobByDateAsync(DateTime date);
            Task<Autobillingjob> GetNextAutoBillingJobByDateAsync(DateTime date);

    }


}
