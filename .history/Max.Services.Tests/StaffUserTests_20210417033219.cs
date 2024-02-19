using System;
using Xunit;
using Max.Core.Models;

namespace Max.Services.Tests
{
    public class StaffUserTests
    {
        [Fact]
        public void Can_Add_New_StaffUser()
        {
            var staffUserService =  new StaffUserService();

        
            StaffUserModel staff =  new StaffUserModel();

            staff.FirstName  =  "ashoks";
            staff.LastName   = "sachan";
            staff.UserName ="ashoks";

            var  newStaff  = staffUserService.CreateStaffUser(staff);

            Assert.True(newStaff.UserId > 0, "Staff User  Created.");

        }
    }
}
