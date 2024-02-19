using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Max.Data.DataModel;

namespace Max.Data.Interfaces
{
    public interface ISessionRepository : IRepository<Session>
    {
        Task<IEnumerable<Session>> GetAllSessionsByEventIdAsync(int eventId);
        Task<Session> GetSessionByIdAsync(int sessionId);
        Task<Session> GetSessionBySessionCodeAsync(string code);
        Task<Session> GetSessionByNameAsync(string name);
        Task<Session> GetSessionByNameAndEventIdAsync(string name, int eventId);
        Task<Session> GetSessionByStartAndEndDateAsync(DateTime startDate, DateTime endDate, int eventId);
    }
}
