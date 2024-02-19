using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;

namespace Max.Data.Interfaces
{
    public interface ILinkEventFeeTypeRepository : IRepository<Linkeventfeetype>
    {
        Task<IEnumerable<Linkeventfeetype>> GetFeeTypesByEventIdAsync(int eventId);
    }
}
