using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Max.Data.DataModel;

namespace Max.Data.Interfaces
{
    public interface ILinkEventGroupRepository : IRepository<Linkeventgroup>
    {
        Task<Linkeventgroup> GetLinkEventGroupByRegistrationGroupIdAndEventIdAsync(int registrationGroupId, int eventId);
        Task<IEnumerable<Linkeventgroup>> GetLinkEventGroupByEventIdAsync(int eventId);
    }
}
