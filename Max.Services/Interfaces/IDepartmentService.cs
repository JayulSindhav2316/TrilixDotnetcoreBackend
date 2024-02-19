using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;
using Max.Core.Models;

namespace Max.Services.Interfaces
{
    public interface IDepartmentService
    {
        Task<IEnumerable<Department>> GetAllDepartments();
        Task<Department> GetDepartmentById(int id);
        Task<Department> CreateDepartment(DepartmentModel departmentModel);
        Task<List<SelectListModel>> GetSelectList();
        Task<bool> UpdateDepartment(DepartmentModel departmentModel);
        Task<bool> DeleteDepartment(int departmentId);

    }
}
