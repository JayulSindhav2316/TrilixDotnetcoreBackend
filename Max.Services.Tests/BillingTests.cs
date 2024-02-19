using Xunit;
using Max.Core.Models;
using Max.Core;
using Max.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using Xunit.Priority;
using AutoMapper;
using Max.Data.DataModel;

namespace Max.Services.Tests
{
    [TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
    public class BillingTests : IClassFixture<DependencySetupFixture>
    {
        private ServiceProvider _serviceProvider;

        public BillingTests(DependencySetupFixture fixture)
        {
            _serviceProvider = fixture.ServiceProvider;
        }

        [Fact]
        public async void CreateBilling_Cycle_Add_New()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {

                var organizationService = scope.ServiceProvider.GetService<IOrganizationService>();
                OrganizationModel organization = TestDataGenerator.GetOrganizationModel();
                var newOrganization = await organizationService.CreateOrganization(organization);

                var membershipPeriodService = scope.ServiceProvider.GetService<IMembershipPeriodService>();
                MembershipPeriodModel periodmodel = TestDataGenerator.GetMembershipPeriodModel();
                var period = await membershipPeriodService.CreateMembershipPeriod(periodmodel);

                var membershipTypeService = scope.ServiceProvider.GetService<IMembershipTypeService>();
                MembershipTypeModel membershiptypemodel = TestDataGenerator.GetMembershipTypeModel();
                var newMembershipType = await membershipTypeService.CreateMembershipType(membershiptypemodel);


                var BillingService = scope.ServiceProvider.GetService<IBillingService>();
                BillingCycleModel model = TestDataGenerator.GetBillingcycle();
                model.MembershipType = new string[] { newMembershipType.MembershipTypeId.ToString() }; ;
                var billingcycle = await BillingService.CreateBillingCycle(model);

                Assert.True(billingcycle.BillingCycleId > 0, "Billing Cycle Created.");
            }

        }

        [Fact]
        public async void CreateBilling_Delete_DeleteCycle()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
            Billingcycle billingcycle = new Billingcycle();
            using (var scope = serviceScopeFactory.CreateScope())
            {

                var organizationService = scope.ServiceProvider.GetService<IOrganizationService>();
                OrganizationModel organization = TestDataGenerator.GetOrganizationModel();
                var newOrganization = await organizationService.CreateOrganization(organization);

                var membershipPeriodService = scope.ServiceProvider.GetService<IMembershipPeriodService>();
                MembershipPeriodModel periodmodel = TestDataGenerator.GetMembershipPeriodModel();
                var period = await membershipPeriodService.CreateMembershipPeriod(periodmodel);

                var membershipTypeService = scope.ServiceProvider.GetService<IMembershipTypeService>();
                MembershipTypeModel membershiptypemodel = TestDataGenerator.GetMembershipTypeModel();
                var newMembershipType = await membershipTypeService.CreateMembershipType(membershiptypemodel);


                var BillingService = scope.ServiceProvider.GetService<IBillingService>();
                BillingCycleModel model = TestDataGenerator.GetBillingcycle();
                model.MembershipType = new string[] { newMembershipType.MembershipTypeId.ToString() }; ;
                billingcycle = await BillingService.CreateBillingCycle(model);

                var billingjob = await BillingService.CreateBillingJob(billingcycle.BillingCycleId);

            }

            // create a new context
            using (var scope = serviceScopeFactory.CreateScope())
            {                
                var BillingService = scope.ServiceProvider.GetService<IBillingService>();
                var mapper = scope.ServiceProvider.GetService<IMapper>();
                var newBillingcycleModel = mapper.Map<BillingCycleModel>(billingcycle);

                await BillingService.DeleteBillingCycle(billingcycle.BillingCycleId);

                var deletedbillingcycle = await BillingService.GetBillingCycleById(billingcycle.BillingCycleId);

                Assert.True(deletedbillingcycle == null, "Billing Cycle Deleted.");
            }
        }

    



        [Fact]
        public async void CreateBilling_Job_Add_New()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {

                var organizationService = scope.ServiceProvider.GetService<IOrganizationService>();
                OrganizationModel organization = TestDataGenerator.GetOrganizationModel();
                var newOrganization = await organizationService.CreateOrganization(organization);

                var membershipPeriodService = scope.ServiceProvider.GetService<IMembershipPeriodService>();
                MembershipPeriodModel periodmodel = TestDataGenerator.GetMembershipPeriodModel();
                var period = await membershipPeriodService.CreateMembershipPeriod(periodmodel);

                var membershipTypeService = scope.ServiceProvider.GetService<IMembershipTypeService>();
                MembershipTypeModel membershiptypemodel = TestDataGenerator.GetMembershipTypeModel();
                var newMembershipType = await membershipTypeService.CreateMembershipType(membershiptypemodel);


                var BillingService = scope.ServiceProvider.GetService<IBillingService>();
                BillingCycleModel billingcyclemodel = TestDataGenerator.GetBillingcycle();
                billingcyclemodel.MembershipType = new string[] { newMembershipType.MembershipTypeId.ToString() }; 
                var billingcycle = await BillingService.CreateBillingCycle(billingcyclemodel);
                
                //BillingJobModel model = TestDataGenerator.GetBillingJob();                
                var billingjob = await BillingService.CreateBillingJob(billingcycle.BillingCycleId);

                Assert.True(billingjob.BillingJobId > 0, "Billing Cycle Created.");
            }

        }


        [Theory, Priority(2)]
        [InlineData(1)]
        [InlineData(0)]
        public async void BillingJob_Get_billingjob_Isdue(int jobstatus)
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {

                var organizationService = scope.ServiceProvider.GetService<IOrganizationService>();
                OrganizationModel organization = TestDataGenerator.GetOrganizationModel();
                var newOrganization = await organizationService.CreateOrganization(organization);

                var membershipPeriodService = scope.ServiceProvider.GetService<IMembershipPeriodService>();
                MembershipPeriodModel periodmodel = TestDataGenerator.GetMembershipPeriodModel();
                var period = await membershipPeriodService.CreateMembershipPeriod(periodmodel);

                var membershipTypeService = scope.ServiceProvider.GetService<IMembershipTypeService>();
                MembershipTypeModel membershiptypemodel = TestDataGenerator.GetMembershipTypeModel();
                var newMembershipType = await membershipTypeService.CreateMembershipType(membershiptypemodel);


                var BillingService = scope.ServiceProvider.GetService<IBillingService>();
                BillingCycleModel billingcyclemodel = TestDataGenerator.GetBillingcycle();
                billingcyclemodel.MembershipType = new string[] { newMembershipType.MembershipTypeId.ToString() };
                var billingcycle = await BillingService.CreateBillingCycle(billingcyclemodel);

                
                var billingjob = await BillingService.CreateBillingJob(billingcycle.BillingCycleId);

                await BillingService.UpdateJobStatus(billingjob.BillingJobId, jobstatus); // 

                var isdue= await BillingService.IsBillingJobDue();                

                if (jobstatus==0)
                { 
                    Assert.True(isdue == false, "job created.");
                }
                else
                {
                    Assert.True(isdue == true, "job pending.");
                }
            }

        }

        [Fact]
        public async void Billing_Get_BillingCycles()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {

                var organizationService = scope.ServiceProvider.GetService<IOrganizationService>();
                OrganizationModel organization = TestDataGenerator.GetOrganizationModel();
                var newOrganization = await organizationService.CreateOrganization(organization);

                var membershipPeriodService = scope.ServiceProvider.GetService<IMembershipPeriodService>();
                MembershipPeriodModel periodmodel = TestDataGenerator.GetMembershipPeriodModel();
                var period = await membershipPeriodService.CreateMembershipPeriod(periodmodel);

                var membershipTypeService = scope.ServiceProvider.GetService<IMembershipTypeService>();
                MembershipTypeModel membershiptypemodel = TestDataGenerator.GetMembershipTypeModel();
                var newMembershipType = await membershipTypeService.CreateMembershipType(membershiptypemodel);


                var BillingService = scope.ServiceProvider.GetService<IBillingService>();
                BillingCycleModel billingcyclemodel = TestDataGenerator.GetBillingcycle();
                billingcyclemodel.MembershipType = new string[] { newMembershipType.MembershipTypeId.ToString() };
                var billingcycle1 = await BillingService.CreateBillingCycle(billingcyclemodel);

                //Add another
                billingcyclemodel = TestDataGenerator.GetBillingcycle();
                billingcyclemodel.MembershipType = new string[] { newMembershipType.MembershipTypeId.ToString() };
                var billingcycle2 = await BillingService.CreateBillingCycle(billingcyclemodel);

                var allBillingcycles = await BillingService.GetBillingCycles(0);

                Assert.True(allBillingcycles.Count() >= 2, "Department return all billing cycles.");

            }

        }


        [Fact]
        public async void Billing_Finzalize_FinzalizeBillingCycle()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {

                var organizationService = scope.ServiceProvider.GetService<IOrganizationService>();
                OrganizationModel organization = TestDataGenerator.GetOrganizationModel();
                var newOrganization = await organizationService.CreateOrganization(organization);

                var membershipPeriodService = scope.ServiceProvider.GetService<IMembershipPeriodService>();
                MembershipPeriodModel periodmodel = TestDataGenerator.GetMembershipPeriodModel();
                var period = await membershipPeriodService.CreateMembershipPeriod(periodmodel);

                var membershipTypeService = scope.ServiceProvider.GetService<IMembershipTypeService>();
                MembershipTypeModel membershiptypemodel = TestDataGenerator.GetMembershipTypeModel();
                var newMembershipType = await membershipTypeService.CreateMembershipType(membershiptypemodel);


                var BillingService = scope.ServiceProvider.GetService<IBillingService>();
                BillingCycleModel billingcyclemodel = TestDataGenerator.GetBillingcycle();
                billingcyclemodel.MembershipType = new string[] { newMembershipType.MembershipTypeId.ToString() };
                var billingcycle = await BillingService.CreateBillingCycle(billingcyclemodel);
                billingcycle.Status = 1;
                

                //BillingJobModel model = TestDataGenerator.GetBillingJob();                
                var billingjob = await BillingService.CreateBillingJob(billingcycle.BillingCycleId);
                billingjob.Status = 1;

                await BillingService.FinzalizeBillingCycle(billingcycle.BillingCycleId);

                await BillingService.UpdateCycleStatus(billingcycle.BillingCycleId, (int)BillingStatus.Finalized);

                Assert.True(billingcycle.Status == (int)BillingStatus.Finalized , "Finalized Billing.");

            }

        }

        [Fact, Priority(3)]
        public async void BillingJob_Get_NextBillingJob()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {

                var organizationService = scope.ServiceProvider.GetService<IOrganizationService>();
                OrganizationModel organization = TestDataGenerator.GetOrganizationModel();
                var newOrganization = await organizationService.CreateOrganization(organization);

                var membershipPeriodService = scope.ServiceProvider.GetService<IMembershipPeriodService>();
                MembershipPeriodModel periodmodel = TestDataGenerator.GetMembershipPeriodModel();
                var period = await membershipPeriodService.CreateMembershipPeriod(periodmodel);

                var membershipTypeService = scope.ServiceProvider.GetService<IMembershipTypeService>();
                MembershipTypeModel membershiptypemodel = TestDataGenerator.GetMembershipTypeModel();
                var newMembershipType = await membershipTypeService.CreateMembershipType(membershiptypemodel);

                var BillingService = scope.ServiceProvider.GetService<IBillingService>();
                BillingCycleModel billingcyclemodel = TestDataGenerator.GetBillingcycle();
                billingcyclemodel.MembershipType = new string[] { newMembershipType.MembershipTypeId.ToString() };
                var billingcycle = await BillingService.CreateBillingCycle(billingcyclemodel);
                
                var billingjob = await BillingService.CreateBillingJob(billingcycle.BillingCycleId);               
                
                await BillingService.UpdateJobStatus(billingjob.BillingJobId, 0); // 0 : for getiing next bill date, 1 : for getting billing job null  

                var newbillingjob = await BillingService.GetNextBillingJob();
                Assert.True(newbillingjob.BillingJobId > 0, "next billing job selected");                
                   
             }

        }


        [Fact, Priority(0)]
        public async void BillingJob_Get_Null_NextBillingJob()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {

                var organizationService = scope.ServiceProvider.GetService<IOrganizationService>();
                OrganizationModel organization = TestDataGenerator.GetOrganizationModel();
                var newOrganization = await organizationService.CreateOrganization(organization);

                var membershipPeriodService = scope.ServiceProvider.GetService<IMembershipPeriodService>();
                MembershipPeriodModel periodmodel = TestDataGenerator.GetMembershipPeriodModel();
                var period = await membershipPeriodService.CreateMembershipPeriod(periodmodel);

                var membershipTypeService = scope.ServiceProvider.GetService<IMembershipTypeService>();
                MembershipTypeModel membershiptypemodel = TestDataGenerator.GetMembershipTypeModel();
                var newMembershipType = await membershipTypeService.CreateMembershipType(membershiptypemodel);

                var BillingService = scope.ServiceProvider.GetService<IBillingService>();
                BillingCycleModel billingcyclemodel = TestDataGenerator.GetBillingcycle();
                billingcyclemodel.MembershipType = new string[] { newMembershipType.MembershipTypeId.ToString() };
                var billingcycle = await BillingService.CreateBillingCycle(billingcyclemodel);
                
               var billingjob= await BillingService.CreateBillingJob(billingcycle.BillingCycleId);

                await BillingService.UpdateJobStatus(billingjob.BillingJobId, 1); // 0 : for getiing next bill date, 1 : for getting billing job null  

                var newbillingjob = await BillingService.GetNextBillingJob();
                Assert.True(newbillingjob.BillingJobId == 0, "No billling Job");
            }

        }

        [Fact]
        public async void BillingJob_Update_UpdateJobStatus()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {

                var organizationService = scope.ServiceProvider.GetService<IOrganizationService>();
                OrganizationModel organization = TestDataGenerator.GetOrganizationModel();
                var newOrganization = await organizationService.CreateOrganization(organization);

                var membershipPeriodService = scope.ServiceProvider.GetService<IMembershipPeriodService>();
                MembershipPeriodModel periodmodel = TestDataGenerator.GetMembershipPeriodModel();
                var period = await membershipPeriodService.CreateMembershipPeriod(periodmodel);

                var membershipTypeService = scope.ServiceProvider.GetService<IMembershipTypeService>();
                MembershipTypeModel membershiptypemodel = TestDataGenerator.GetMembershipTypeModel();
                var newMembershipType = await membershipTypeService.CreateMembershipType(membershiptypemodel);

                var BillingService = scope.ServiceProvider.GetService<IBillingService>();
                BillingCycleModel billingcyclemodel = TestDataGenerator.GetBillingcycle();
                billingcyclemodel.MembershipType = new string[] { newMembershipType.MembershipTypeId.ToString() };
                var billingcycle = await BillingService.CreateBillingCycle(billingcyclemodel);                

                var billingjob = await BillingService.CreateBillingJob(billingcycle.BillingCycleId);

                var updatedjob= await BillingService.UpdateJobStatus(billingjob.BillingJobId, 3); // 

                
                Assert.True(billingjob.Status == 3, "next billing job selected");

            }

        }


        [Fact]
        public async void BillingJob_Update_UpdateCycleStatus()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {

                var organizationService = scope.ServiceProvider.GetService<IOrganizationService>();
                OrganizationModel organization = TestDataGenerator.GetOrganizationModel();
                var newOrganization = await organizationService.CreateOrganization(organization);

                var membershipPeriodService = scope.ServiceProvider.GetService<IMembershipPeriodService>();
                MembershipPeriodModel periodmodel = TestDataGenerator.GetMembershipPeriodModel();
                var period = await membershipPeriodService.CreateMembershipPeriod(periodmodel);

                var membershipTypeService = scope.ServiceProvider.GetService<IMembershipTypeService>();
                MembershipTypeModel membershiptypemodel = TestDataGenerator.GetMembershipTypeModel();
                var newMembershipType = await membershipTypeService.CreateMembershipType(membershiptypemodel);

                var BillingService = scope.ServiceProvider.GetService<IBillingService>();
                BillingCycleModel billingcyclemodel = TestDataGenerator.GetBillingcycle();
                billingcyclemodel.MembershipType = new string[] { newMembershipType.MembershipTypeId.ToString() };
                var billingcycle = await BillingService.CreateBillingCycle(billingcyclemodel);

                await BillingService.UpdateCycleStatus(billingcycle.BillingCycleId, 2);               

                Assert.True(billingcycle.Status >= 2, "next billing job selected");

            }

        }

        [Fact]        
        public async void BillingJob_Get_NextBillingFinalizationJob()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {

                var organizationService = scope.ServiceProvider.GetService<IOrganizationService>();
                OrganizationModel organization = TestDataGenerator.GetOrganizationModel();
                var newOrganization = await organizationService.CreateOrganization(organization);

                var membershipPeriodService = scope.ServiceProvider.GetService<IMembershipPeriodService>();
                MembershipPeriodModel periodmodel = TestDataGenerator.GetMembershipPeriodModel();
                var period = await membershipPeriodService.CreateMembershipPeriod(periodmodel);

                var membershipTypeService = scope.ServiceProvider.GetService<IMembershipTypeService>();
                MembershipTypeModel membershiptypemodel = TestDataGenerator.GetMembershipTypeModel();
                var newMembershipType = await membershipTypeService.CreateMembershipType(membershiptypemodel);

                var BillingService = scope.ServiceProvider.GetService<IBillingService>();
                BillingCycleModel billingcyclemodel = TestDataGenerator.GetBillingcycle();
                billingcyclemodel.MembershipType = new string[] { newMembershipType.MembershipTypeId.ToString() };
                var billingcycle = await BillingService.CreateBillingCycle(billingcyclemodel);

                await BillingService.UpdateCycleStatus(billingcycle.BillingCycleId, 1);
                
                var billingjob = await BillingService.CreateBillingJob(billingcycle.BillingCycleId);                

                await BillingService.UpdateJobStatus(billingjob.BillingJobId, 1); // 0 : for getiing next bill date, 1 : for getting billing job null 

                await BillingService.FinzalizeBillingCycle(billingcycle.BillingCycleId);

                var newbillingjob = await BillingService.GetNextBillingFinalizationJob();
                Assert.True(newbillingjob.BillingJobId > 0, "next billing job selected");

            }

        }


        [Fact]
        public async void BillingJob_Get_BillingCycle_ById()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {

                var organizationService = scope.ServiceProvider.GetService<IOrganizationService>();
                OrganizationModel organization = TestDataGenerator.GetOrganizationModel();
                var newOrganization = await organizationService.CreateOrganization(organization);

                var membershipPeriodService = scope.ServiceProvider.GetService<IMembershipPeriodService>();
                MembershipPeriodModel periodmodel = TestDataGenerator.GetMembershipPeriodModel();
                var period = await membershipPeriodService.CreateMembershipPeriod(periodmodel);

                var membershipTypeService = scope.ServiceProvider.GetService<IMembershipTypeService>();
                MembershipTypeModel membershiptypemodel = TestDataGenerator.GetMembershipTypeModel();
                var newMembershipType = await membershipTypeService.CreateMembershipType(membershiptypemodel);


                var BillingService = scope.ServiceProvider.GetService<IBillingService>();
                BillingCycleModel billingcyclemodel = TestDataGenerator.GetBillingcycle();
                billingcyclemodel.MembershipType = new string[] { newMembershipType.MembershipTypeId.ToString() };
                var billingcycle = await BillingService.CreateBillingCycle(billingcyclemodel);

                var newbillingcycle= await BillingService.GetBillingCycleById(billingcycle.BillingCycleId);

                Assert.True(newbillingcycle.BillingCycleId == billingcycle.BillingCycleId , "Department returns selected Id.");
                
            }

        }


    }
}