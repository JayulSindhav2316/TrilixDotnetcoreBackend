using System;
using System.Threading.Tasks;

namespace Max.Data.Interfaces
{
    public interface ITenantUnitOfWork : IDisposable
    {
        ITenantRepository Tenants { get; }
      
        Task<int> CommitAsync();
    }
}