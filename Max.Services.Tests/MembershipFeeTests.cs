using System;
using Xunit;
using Max.Core.Models;
using Max.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Max.Core;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Max.Services.Tests
{
    public class MembershipFeeTests : IClassFixture<DependencySetupFixture>
    {
        private ServiceProvider _serviceProvider;

        public MembershipFeeTests(DependencySetupFixture fixture)
        {
            _serviceProvider = fixture.ServiceProvider;
        }

        [Fact]
        public async void CreateMembershipFee_Add_New()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var membershipFeeService = scope.ServiceProvider.GetService<IMembershipFeeService>();

                MembershipFeeModel model = TestDataGenerator.GetMembershipFeeModel();

                var newmembershipFee = await membershipFeeService.CreateMembershipFee(model);
                Assert.True(newmembershipFee.FeeId > 0, "Membership Fee Created.");
            }

        }

        [Fact]
        public async void CreatemembershipFee_Update()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var membershipFeeService = scope.ServiceProvider.GetService<IMembershipFeeService>();

                MembershipFeeModel model = TestDataGenerator.GetMembershipFeeModel();

                var newmembershipFee = await membershipFeeService.CreateMembershipFee(model);
                model.Name = "Changed";
                model.FeeId = newmembershipFee.FeeId;

                var updatedmembershipFee = await membershipFeeService.UpdateMembershipFee(model);
                Assert.True(updatedmembershipFee.Name == "Changed", "Membership Fee Updated.");
            }

        }


        [Fact]
        public async void CreatemembershipFee_withMembershiptypeID_Delete()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {

                 var membershipTypeService = scope.ServiceProvider.GetService<IMembershipTypeService>();                
                 MembershipTypeModel membershiptype = TestDataGenerator.GetMembershipTypeModel();
                 var newmembershiptype = await membershipTypeService.CreateMembershipType(membershiptype);


                var membershipFeeService = scope.ServiceProvider.GetService<IMembershipFeeService>();
                MembershipFeeModel model = TestDataGenerator.GetMembershipFeeModel();
                var newMembershipFee = await membershipFeeService.CreateMembershipFee(model);

                newMembershipFee.MembershipTypeId = newmembershiptype.MembershipTypeId;
               

                var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => membershipFeeService.DeleteMembershipFee(newMembershipFee.FeeId));

                Assert.IsType<InvalidOperationException>(ex);
                Assert.Equal($"You cannot delete this fee.", ex.Message);
                
            }

        }

        [Fact]
        public async void Get_MembershipFee_GetAll()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {

                var membershipTypeService = scope.ServiceProvider.GetService<IMembershipTypeService>();
                MembershipTypeModel membershiptype = TestDataGenerator.GetMembershipTypeModel();
                var newmembershiptype = await membershipTypeService.CreateMembershipType(membershiptype);


                var membershipFeeService = scope.ServiceProvider.GetService<IMembershipFeeService>();
                MembershipFeeModel model = TestDataGenerator.GetMembershipFeeModel();
                await membershipFeeService.CreateMembershipFee(model);

                
               var membershipfeerecords= await membershipFeeService.GetAllMembershipFees();


                Assert.True(!Extenstions.IsNullOrEmpty(membershipfeerecords), "Membershipfees has records.");
                
            }

        }

        [Fact]
        public async void Get_MembershipFee_ByMembershipTypeId()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {

                var membershipTypeService = scope.ServiceProvider.GetService<IMembershipTypeService>();
                MembershipTypeModel membershiptype = TestDataGenerator.GetMembershipTypeModel();
                var newmembershiptype = await membershipTypeService.CreateMembershipType(membershiptype);

                var membershipFeeService = scope.ServiceProvider.GetService<IMembershipFeeService>();
                MembershipFeeModel model = TestDataGenerator.GetMembershipFeeModel();
                await membershipFeeService.CreateMembershipFee(model);                

                var membershipfeesrecord = await membershipFeeService.GetMembershipFeesByMembershipTypeId(newmembershiptype.MembershipTypeId);
                var selectedrecord= membershipfeesrecord.Select(p => p.MembershipTypeId=newmembershiptype.MembershipTypeId).ToList();

                Assert.True(selectedrecord.Count()> 0, "Membershipfees returns selected Id.");

            }

        }

        [Fact]
        public async void Get_MembershipFee_ByFeeId()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {           

                var membershipFeeService = scope.ServiceProvider.GetService<IMembershipFeeService>();
                MembershipFeeModel model = TestDataGenerator.GetMembershipFeeModel();
                var newmembershipfee= await membershipFeeService.CreateMembershipFee(model);


                var membershipfeerecords = await membershipFeeService.GetMembershipFeeById(newmembershipfee.FeeId);

                Assert.True(membershipfeerecords.FeeId == newmembershipfee.FeeId, "Membershipfees returns selected Id.");               

            }

        }

        [Fact]
        public async void Get_MembershipFee_ByFeeIds()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {

                var membershipFeeService = scope.ServiceProvider.GetService<IMembershipFeeService>();
                MembershipFeeModel model = TestDataGenerator.GetMembershipFeeModel();
                var newmembershipfee1 = await membershipFeeService.CreateMembershipFee(model);

                //Add Another
                model= TestDataGenerator.GetMembershipFeeModel();
                var newmembershipfee2 = await membershipFeeService.CreateMembershipFee(model);

                //Add Another
                model = TestDataGenerator.GetMembershipFeeModel();
                var newmembershipfee3 = await membershipFeeService.CreateMembershipFee(model);

                var membershipfeerecords = await membershipFeeService.GetMembershipFeesByFeeIds((newmembershipfee1.FeeId.ToString() + "," + newmembershipfee2.FeeId.ToString() + "," + newmembershipfee3.FeeId.ToString()).ToString());

                Assert.True(!Extenstions.IsNullOrEmpty(membershipfeerecords), "GlAccount has records.");                

            }

        }

        [Fact]
        public async void CreatemembershipFee_withoutMembershiptypeID_Delete()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {

               var membershipFeeService = scope.ServiceProvider.GetService<IMembershipFeeService>();

                MembershipFeeModel model = TestDataGenerator.GetMembershipFeeModel();

                var newMembershipFee = await membershipFeeService.CreateMembershipFee(model);                

                await membershipFeeService.DeleteMembershipFee(newMembershipFee.FeeId);
                var deletedMembershipFee = await membershipFeeService.GetMembershipFeeById(newMembershipFee.FeeId);
                Assert.True(deletedMembershipFee == null, "membershipFee Deleted.");
            }

        }

    }
}