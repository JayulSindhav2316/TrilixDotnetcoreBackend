using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Max.Data.DataModel;

namespace Max.Data.Interfaces
{
    public interface IEventRepository : IRepository<Event>
    {
        Task<IEnumerable<Event>> GetAllEventsAsync();
        Task<IEnumerable<Event>> GetEventsByStatusAsync(int status);
        Task<IEnumerable<Event>> GetUpcomingEventsAsync(DateTime startDate);
        Task<IEnumerable<Event>> GetPastEventsAsync(DateTime endDate);
        Task<Event> GetEventByIdAsync(int eventId);
        Task<Event> GetEventByEventCodeAsync(string code);
        Task<Event> GetEventByEventNameAsync(string name);
    }
}
