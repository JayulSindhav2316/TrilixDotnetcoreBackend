using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Max.Data.DataModel;

namespace Max.Data.Interfaces
{
    public interface IEventContactRepository : IRepository<Eventcontact>
    {
        Task<IEnumerable<Eventcontact>> GetEventContactsByEventIdAsync(int eventId);
    }
}
