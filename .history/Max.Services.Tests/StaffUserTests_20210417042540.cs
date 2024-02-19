using System;
using Xunit;
using Max.Core.Models;
using Max.Data.DataModel;
using Max.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Max.Services.Tests
{
    public class StaffUserTests : IClassFixture<DependencySetupFixture>
    {
        private ServiceProvider _serviceProvide;

        public StaffUserTests(DependencySetupFixture fixture)
        {
            _serviceProvide = fixture.ServiceProvider;
        }

        [Fact]
        public void Can_Add_New_StaffUser()
        {
             using(var scope = _serviceProvider.CreateScope())
                {   
                    // Arrange
                  var staffUserService = scope.ServiceProvider.GetServices<IStaffUserService>();

                    StaffUserModel staff =  new StaffUserModel();

                    staff.FirstName  =  "ashoks";
                    staff.LastName   = "sachan";
                    staff.UserName ="ashoks";

                    var  newStaff  = staffUserService.CreateStaffUser(staff);    
                      Assert.True(newStaff.UserId > 0, "Staff User  Created."); 
                }

        
        }
    }
    public class DependencySetupFixture
    {
        public DependencySetupFixture()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddDbContext<membermaxContext>(options => options.UseInMemoryDatabase(databaseName: "TestDatabase"));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IStaffUserService, StaffUserService>();

            ServiceProvider = serviceCollection.BuildServiceProvider();
        }

        public ServiceProvider ServiceProvider { get; private set; }
    }
    
}
