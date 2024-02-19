using Max.Data.DataModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Max.Data.Interfaces
{
    public interface IEventRegisterRepository : IRepository<Eventregister>
    {
        Task<IEnumerable<Eventregister>> GetAllEventRegistrationsByEventIdAsync(int eventId);
    }
}
