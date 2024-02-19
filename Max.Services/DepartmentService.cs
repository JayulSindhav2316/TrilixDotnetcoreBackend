using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.Repositories;
using Max.Data.Interfaces;
using Max.Data.DataModel;
using Max.Services.Interfaces;
using Max.Core.Models;
using Max.Core;
using Max.Data;
using Max.Services;
using System.Linq;
namespace Max.Services
{
    public class DepartmentService : IDepartmentService
    {

        private readonly IUnitOfWork _unitOfWork;
        public DepartmentService(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Department>> GetAllDepartments()
        {
            return await _unitOfWork.Departments
                .GetAllDepartmentsAsync();
        }

        public async Task<Department> GetDepartmentById(int id)
        {
            return await _unitOfWork.Departments
                .GetDepartmentByIdAsync(id);
        }
        public async Task<Department> CreateDepartment(DepartmentModel departmentModel)
        {
            Department department = new Department();

            var organization = await _unitOfWork.Organizations.GetAllOrganizationsAsync();

            if (organization != null)
            {         
                if (organization.Any()) 
                { 
                    departmentModel.OrganizationId = organization.FirstOrDefault().OrganizationId;
                }
            }
            else
            {
                departmentModel.OrganizationId = 0;
            }

            var isValid = await ValidDepartment(departmentModel);
            if (isValid)
            {
                //Map Model Data
                department.Name = departmentModel.Name;
                department.Status = departmentModel.Status;
                department.OrganizationId = departmentModel.OrganizationId;
                department.CostCenterCode = departmentModel.CostCenterCode;
                department.Description = departmentModel.Description;
                await _unitOfWork.Departments.AddAsync(department);
                await _unitOfWork.CommitAsync();
            }
            return department;
        }

        public async Task<List<SelectListModel>> GetSelectList()
        {
            var departments = await _unitOfWork.Departments.GetAllDepartmentsAsync();

            List<SelectListModel> selectList = new List<SelectListModel>();
            foreach (var department in departments)
            {
                SelectListModel selectListItem = new SelectListModel();
                selectListItem.code = department.DepartmentId.ToString();
                selectListItem.name = department.Name;
                if(department.Status == 1)
                {
                    selectList.Add(selectListItem);
                }                
            }
            return selectList;
        }

        public async Task<bool> UpdateDepartment(DepartmentModel departmentModel)
        {
            var isValidDepartment = await ValidDepartment (departmentModel);
            if (isValidDepartment)
            {
                var department = await _unitOfWork.Departments.GetByIdAsync(departmentModel.DepartmentId);

                if (department != null)
                {
                    department.Name = departmentModel.Name;
                    department.Description = departmentModel.Description;
                    department.Status = departmentModel.Status;
                    department.CostCenterCode = departmentModel.CostCenterCode;
                }
                _unitOfWork.Departments.Update(department);
                await _unitOfWork.CommitAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteDepartment(int id)
        {
 
            var department = await _unitOfWork.Departments.GetByIdAsync(id);
            if (department != null)
            {
                _unitOfWork.Departments.Remove(department);
                await _unitOfWork.CommitAsync();
                return true;
            }
            return false;

        }
        private async Task<bool> ValidDepartment(DepartmentModel model)
        {
            var department = new Department();
            //Validate  Name
            if (model.Name == null)
            {
                throw new NullReferenceException($"Department Name can not be NULL.");
            }
            if (model.Name.Trim() == String.Empty)
            {
                throw new InvalidOperationException($"Department Name can not be empty.");
            }            

            //Check if Department already exists

            department = await _unitOfWork.Departments.GetDepartmentByNameAsync(model.Name);

            if (department != null)
            {
                //check id ID & UserName are same -> Updating
                if (department.DepartmentId != model.DepartmentId)
                {
                    if (department.Name == model.Name)
                    {
                        throw new InvalidOperationException($"Department name already exists.");
                    }
                }
            }

            department = await _unitOfWork.Departments.GetDepartmentByCodeAsync(model.CostCenterCode);

            if (department != null)
            {                
                if (department.DepartmentId != model.DepartmentId)                
                {                    
                    if (department.CostCenterCode == model.CostCenterCode)
                    {
                        throw new InvalidOperationException($"Cost Center Code already exists.");
                    }
                }
            }

            return true;
        }


    }
}
