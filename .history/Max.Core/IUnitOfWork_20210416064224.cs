using System;
using System.Threading.Tasks;
using Max.Core.Repositories;

namespace MyMusic.Core
{
    public interface IUnitOfWork : IDisposable
    {
        IStafffUserRepository StaffUsers { get; }
        Task<int> CommitAsync();
    }
}