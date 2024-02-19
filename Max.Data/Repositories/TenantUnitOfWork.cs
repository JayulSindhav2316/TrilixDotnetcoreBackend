using System.Threading.Tasks;
using Max.Data.Interfaces;
using Max.Data.Repositories;
using Max.Data.DataModel;

namespace Max.Data
{
    public class TenantUnitOfWork : ITenantUnitOfWork
    {
        private readonly maxtenantContext _context;
        private TenantRepository _tenantRepository;
        public TenantUnitOfWork(maxtenantContext context)
        {
            this._context = context;
        }

        public ITenantRepository Tenants => _tenantRepository = _tenantRepository ?? new TenantRepository(_context);
              public async Task<int> CommitAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}