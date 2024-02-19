using Max.Data.DataModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Max.Data.Interfaces
{
    public interface IGroupRegistrationRepository: IRepository<Registrationgroup>
    {
        Task<bool> RegisterGroup(Registrationgroup group);
        Task<List<Registrationgroup>> GetAll(string searchText);

        Task<bool> LinkMembership(Registrationgroupmembershiplink group);

        Task<bool> DeleteLink(int linkId);
        Task<List<Registrationgroupmembershiplink>> GetLinkTypesByGroupId(int groupId);
        Task<Registrationgroup> GetGroupByNameAsync(string groupName);
    }
}
