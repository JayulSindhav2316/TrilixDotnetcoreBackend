using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;
using Max.Core.Models;

namespace Max.Services.Interfaces
{
    public interface IMembershipConnectionService
    {        
        Task<Membershipconnection> CreateMembershipConnection(MembershipConnectionModel MembershipconnectionModel);
        Task<IEnumerable<Membershipconnection>> GetMembershipConnectionsByMembershipId(int membershipId);


    }
}
