using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Max.Data.Interfaces;
using Max.Data.Repositories;
using Max.Data.DataModel;

namespace Max.Data.Repositories
{
    public class DepartmentRepository : Repository<Department>, IDepartmentRepository
    {
        public DepartmentRepository(membermaxContext context)
            : base(context)
        { }

        public async Task<IEnumerable<Department>> GetAllDepartmentsAsync()
        {
            return await membermaxContext.Departments
                .ToListAsync();
        }

        public async Task<Department> GetDepartmentByIdAsync(int id)
        {
            return await membermaxContext.Departments
                .SingleOrDefaultAsync(m => m.DepartmentId == id);
        }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }

        public async Task<Department> GetDepartmentByNameAsync(string name)
        {
            return await membermaxContext.Departments
                .FirstOrDefaultAsync(m => m.Name == name);
        }
        public async Task<Department> GetDepartmentByCodeAsync(string code)
        {
            return await membermaxContext.Departments
                .FirstOrDefaultAsync(m => m.CostCenterCode == code);
        }

    }
}
