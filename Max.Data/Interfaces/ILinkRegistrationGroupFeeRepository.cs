using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Max.Data.DataModel;

namespace Max.Data.Interfaces
{
    public interface ILinkRegistrationGroupFeeRepository : IRepository<Linkregistrationgroupfee>
    {
        Task<IEnumerable<Linkregistrationgroupfee>> GetLinkRegistrationGroupFeesByLinkEventGroupIdAsync(int linkEventgroupId);
    }
}
