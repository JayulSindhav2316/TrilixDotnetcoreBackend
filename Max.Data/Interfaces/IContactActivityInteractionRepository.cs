using Max.Data.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Max.Data.Interfaces
{
    public interface IContactActivityInteractionRepository: IRepository<Contactactivityinteraction>
    {
        Task<Contactactivityinteraction> GetByAccountContactActivityIdAsync(int id, int? accountId, int? contactId);
        Task<Contactactivityinteraction> GetByAccountContactIdAsync(int? accountId, int? contactId);
    }
}
