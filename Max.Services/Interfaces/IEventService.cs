using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Max.Core.Models;

namespace Max.Services.Interfaces
{
    public interface IEventService
    {
        Task<EventModel> CreateEvent(EventModel eventModel);
        Task<bool> UpdateEvent(EventModel model);
        Task<bool> UpdateEventSettings(EventModel model);
        Task<IEnumerable<EventModel>> GetAllEvents();
        Task<EventModel> GetEventDetailsById(int eventId);
        Task<EventModel> GetEventBasicDetailsById(int eventId);
        Task<bool> DeleteEvent(int eventId);
        Task<bool> CloneEvent(int eventId);
        Task<IEnumerable<EventListModel>> GetEventsByFilter(DateTime date, int filter = 1);
        Task<IEnumerable<EventModel>> GetAllActiveEvents(bool includePastEvents);
        Task<int> CreateEventRegister(EventRegisterModel eventRegisterModel);
        Task<bool> CheckEventRegistrationByEventId(int eventId);
    }
}
