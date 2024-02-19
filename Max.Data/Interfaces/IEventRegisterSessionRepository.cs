using Max.Data.DataModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Max.Data.Interfaces
{
    public interface IEventRegisterSessionRepository : IRepository<Eventregistersession>
    {
        Task<List<Eventregistersession>> GetRegisteredSessionsBySessionIdAsync(int sessionId);
        Task<Eventregistersession> GetRegisteredSessionsByEntity(int eventId, int entityId, int sessionId);
    }
}
