using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;


namespace Max.Data.Interfaces
{
    public interface IDepartmentRepository : IRepository<Department>
    {
        Task<IEnumerable<Department>> GetAllDepartmentsAsync();
        Task<Department> GetDepartmentByIdAsync(int id);
        Task<Department> GetDepartmentByNameAsync(string name);
        Task<Department> GetDepartmentByCodeAsync(string code);
    }
}
