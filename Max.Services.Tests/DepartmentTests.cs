using Xunit;
using Max.Core.Models;
using Max.Core;
using Max.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Max.Services.Tests
{
    public class DepartmentTests : IClassFixture<DependencySetupFixture>
    {
        private ServiceProvider _serviceProvider;

        public DepartmentTests(DependencySetupFixture fixture)
        {
            _serviceProvider = fixture.ServiceProvider;
        }

        [Fact]
        public async void CreateDepartment_Add_Newl()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {

                var organizationService = scope.ServiceProvider.GetService<IOrganizationService>();
                OrganizationModel organization = TestDataGenerator.GetOrganizationModel();
                var newOrganization = await organizationService.CreateOrganization(organization);

                var DepartmentService = scope.ServiceProvider.GetService<IDepartmentService>();

                DepartmentModel model = TestDataGenerator.GetDepartmentModel();

                var category = await DepartmentService.CreateDepartment(model);
                Assert.True(category.DepartmentId > 0, "Department Created.");
            }

        }


        [Fact]
        public async void Department_Update()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {

                var organizationService = scope.ServiceProvider.GetService<IOrganizationService>();
                OrganizationModel organization = TestDataGenerator.GetOrganizationModel();
                var newOrganization = await organizationService.CreateOrganization(organization);

                var DepartmentService = scope.ServiceProvider.GetService<IDepartmentService>();
                DepartmentModel model = TestDataGenerator.GetDepartmentModel();
                var category = await DepartmentService.CreateDepartment(model);

                model.DepartmentId = category.DepartmentId;
                model.Name = "Changed Name";
                model.Description = "Changed Description";

                await DepartmentService.UpdateDepartment(model);

                Assert.True(category.Name == "Changed Name", "Department updated.");
                Assert.True(category.Description == "Changed Description", "Department updated.");
            }

        }

        [Fact]
        public async void Department_Delete()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {

                var organizationService = scope.ServiceProvider.GetService<IOrganizationService>();
                OrganizationModel organization = TestDataGenerator.GetOrganizationModel();
                var newOrganization = await organizationService.CreateOrganization(organization);

                var DepartmentService = scope.ServiceProvider.GetService<IDepartmentService>();
                DepartmentModel model = TestDataGenerator.GetDepartmentModel();
                var category = await DepartmentService.CreateDepartment(model);

                await DepartmentService.DeleteDepartment(model.DepartmentId);
                var deleteddepartment= await DepartmentService.GetDepartmentById(model.DepartmentId);

                Assert.True(deleteddepartment == null, "Department Deleted.");

            }

        }


        [Fact]
        public async void Craeate_Department_withblankname()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {

                var organizationService = scope.ServiceProvider.GetService<IOrganizationService>();
                OrganizationModel organization = TestDataGenerator.GetOrganizationModel();
                var newOrganization = await organizationService.CreateOrganization(organization);

                var DepartmentService = scope.ServiceProvider.GetService<IDepartmentService>();
                DepartmentModel model = TestDataGenerator.GetDepartmentModel_WithBlankName();
                //var category = await DepartmentService.CreateDepartment(model);          
                
                var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => DepartmentService.CreateDepartment(model));
                
                Assert.IsType<InvalidOperationException>(ex);
                Assert.Contains("Department Name can not be empty.", ex.Message);

            }

        }

        [Fact]
        public async void Craeate_Department_withnullasname()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {

                var organizationService = scope.ServiceProvider.GetService<IOrganizationService>();
                OrganizationModel organization = TestDataGenerator.GetOrganizationModel();
                var newOrganization = await organizationService.CreateOrganization(organization);

                var DepartmentService = scope.ServiceProvider.GetService<IDepartmentService>();
                DepartmentModel model = TestDataGenerator.GetDepartmentModel_WithnullName();
                //var category = await DepartmentService.CreateDepartment(model);          

                var ex = await Assert.ThrowsAsync<NullReferenceException>(() => DepartmentService.CreateDepartment(model));

                Assert.IsType<NullReferenceException>(ex);
                Assert.Contains("Department Name can not be NULL.", ex.Message);

            }

        }


        [Theory]        
        [InlineData("Duplicatedetartment")]       
        
        public async void Craeate_Department_withduplicatename(string deptname)
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {

                var organizationService = scope.ServiceProvider.GetService<IOrganizationService>();
                OrganizationModel organization = TestDataGenerator.GetOrganizationModel();
                var newOrganization = await organizationService.CreateOrganization(organization);

                var DepartmentService = scope.ServiceProvider.GetService<IDepartmentService>();
                DepartmentModel model = new DepartmentModel();
                model.Name = deptname;
                model.Description = "Description";
                model.CostCenterCode = "ABC";
                model.Status = 1;
                
                var department1 = await DepartmentService.CreateDepartment(model);

                //add another department with same name
                DepartmentModel duplicatemodel = new DepartmentModel();
                duplicatemodel.Name = deptname;
                duplicatemodel.Description = "different Description";
                duplicatemodel.CostCenterCode = "XYZ";
                duplicatemodel.Status = 1;

                var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => DepartmentService.CreateDepartment(duplicatemodel));

                Assert.IsType<InvalidOperationException>(ex);
                Assert.Contains("Department name already exists.", ex.Message);

            }

        }


        [Fact]
        public async void Department_GetAll()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {

                var organizationService = scope.ServiceProvider.GetService<IOrganizationService>();
                OrganizationModel organization = TestDataGenerator.GetOrganizationModel();
                var newOrganization = await organizationService.CreateOrganization(organization);

                var DepartmentService = scope.ServiceProvider.GetService<IDepartmentService>();

                DepartmentModel model = TestDataGenerator.GetDepartmentModel();

                await DepartmentService.CreateDepartment(model);

                //Add another

                model = TestDataGenerator.GetDepartmentModel();

                await DepartmentService.CreateDepartment(model);

                var categories = await DepartmentService.GetAllDepartments();

                Assert.True(!Extenstions.IsNullOrEmpty(categories), "Department has records.");
            }

        }

        [Fact]
        public async void Department_GetById()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {

                var organizationService = scope.ServiceProvider.GetService<IOrganizationService>();
                OrganizationModel organization = TestDataGenerator.GetOrganizationModel();
                var newOrganization = await organizationService.CreateOrganization(organization);


                var DepartmentService = scope.ServiceProvider.GetService<IDepartmentService>();

                DepartmentModel model = TestDataGenerator.GetDepartmentModel();

                var category = await DepartmentService.CreateDepartment(model);

                var returnedCategory = await DepartmentService.GetDepartmentById(category.DepartmentId);

                Assert.True(returnedCategory.DepartmentId == category.DepartmentId, "Department returns selected Id.");
            }

        }

        [Fact]
        public async void Department_GetSelectList()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {

                var organizationService = scope.ServiceProvider.GetService<IOrganizationService>();
                OrganizationModel organization = TestDataGenerator.GetOrganizationModel();
                var newOrganization = await organizationService.CreateOrganization(organization);

                var DepartmentService = scope.ServiceProvider.GetService<IDepartmentService>();

                DepartmentModel model = TestDataGenerator.GetDepartmentModel();

                await DepartmentService.CreateDepartment(model);

                //Add another

                model = TestDataGenerator.GetDepartmentModel();

                await DepartmentService.CreateDepartment(model);

                var selectList = await DepartmentService.GetSelectList();

                Assert.True(!Extenstions.IsNullOrEmpty(selectList), "Department returns select List.");
            }

        }


    }
}