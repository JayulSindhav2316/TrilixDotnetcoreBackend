using System;
using System.Threading.Tasks;

namespace Max.Data.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IStaffUserRepository Staffusers { get; }
        Task<int> CommitAsync();
    }
}