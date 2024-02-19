using Max.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Max.Services.Interfaces
{
    public interface IGroupRegistrationService
    {
        Task<bool> RegisterGroup(RegistrationGroupModel group);
        Task<List<Data.DataModel.Registrationgroup>> GetRegisterGroups(string searchText);
        Task<bool> DeleteGroup(int id);
        Task<bool> UpdateGroup(RegistrationGroupModel group);
        Task<bool> LinkMembership(RegistrationGroupModel group);
        Task<bool> DeleteLink(int linkId);
        Task<IEnumerable<LinkEventGroupModel>> GetLinkEventModelForAllRegisterGroups(int eventId);
    }
}
