using System;
using Xunit;
using Max.Core.Models;
using Max.Data.DataModel;
using Max.Services.Interfaces;

namespace Max.Services.Tests
{
    public class StaffUserTests
    {
        [Fact]
        public void Can_Add_New_StaffUser()
        {
            var services = new ServiceCollection();
            services.AddDbContext<membermaxContext>(a => a.UseInMemoryDatabase("Test"));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IStaffUserService, StaffUserService>();

            ServiceProvider serviceProvider = services.BuildServiceProvider();
            var scope = serviceProvider.CreateScope();

            var testdbContext = scope.ServiceProvider.GetServices<membermaxContext>();
            var unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();


            var staffUserService =  scope.ServiceProvider.GetService<IStaffUserServicecs<StaffUserModel>>();

        
            StaffUserModel staff =  new StaffUserModel();

            staff.FirstName  =  "ashoks";
            staff.LastName   = "sachan";
            staff.UserName ="ashoks";

            var  newStaff  = staffUserService.CreateStaffUser(staff);

            Assert.True(newStaff.UserId > 0, "Staff User  Created.");

        }
    }
}
