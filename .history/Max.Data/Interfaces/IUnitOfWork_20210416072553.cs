using System;
using System.Threading.Tasks;
using Max.Core.Repositories;

namespace Max.Core
{
    public interface IUnitOfWork : IDisposable
    {
        IStaffUserRepository Staffusers { get; }
        Task<int> CommitAsync();
    }
}