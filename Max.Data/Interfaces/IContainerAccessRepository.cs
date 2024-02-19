using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;

namespace Max.Data.Interfaces
{
    public interface IContainerAccessRepository : IRepository<Containeraccess>
    {

        Task<IEnumerable<Containeraccess>> GetAllContainerAccessAsync();
        Task<IEnumerable<Containeraccess>> GetContainerAccessByContainerIdAsync(int id);
        Task<Containeraccess> GetContainerAccessByIdAsync(int id);
        Task<IEnumerable<Containeraccess>> GetContainerAccessByMembershipTypeIdAsync(int id);
        Task<IEnumerable<Containeraccess>> GetContainerAccessByGroupIdAsync(int id);

    }
}
