using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Max.Core.Models;
using Max.Data.DataModel;

namespace Max.Services.Interfaces
{
    public interface ISessionService
    {
        Task<IEnumerable<SessionModel>> GetAllSessionsByEventId(int sessionId);
        Task<SessionModel> CreateSession(SessionModel sessionModel);
        Task<bool> UpdateSession(SessionModel model);
        Task<SessionModel> GetSessionById(int sessionId);
        Task<bool> DeleteSession(int sessionId);
        Task<SessionModel> GetNewSessionModel(int eventId);
        Task<bool> CloneSession(int sessionId, int eventId = 0);
        Task<IEnumerable<SessionModel>> GetSessionLeadersBySessionId(string documentRoot, string tenantId, int eventId);
        Task<IEnumerable<EventRegistrationSessionGroupAndPricingModel>> GetEventRegistrationSessionGroupAndPricing(int eventId, int entityId);
        Task<IEnumerable<string>> GetRegisteredSessionsByEntity(int eventId, int entityId, string sessionIds);
        Task<IEnumerable<Eventregistersession>> GetRegisteredSessions(int sessionId);
    }
}
