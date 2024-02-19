using System.Threading.Tasks;
using MyMusic.Core;
using Max.Core.Repositories;
using Max.Data.Repositories;
using  Max.Data.DataModel;

namespace Max.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly membermaxContext _context;
        private StaffUserRepository _staffUserRepository;

        public UnitOfWork(membermaxContext context)
        {
            this._context = context;
        }

        public IStaffUserRepository Staffusers => _staffUserRepository = _staffUserRepository ?? new StaffUserRepository(_context);

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