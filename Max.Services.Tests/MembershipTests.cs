using Xunit;
using Max.Core.Models;
using Max.Core;
using Max.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using System.Linq;
using Max.Data.DataModel;
using System;

namespace Max.Services.Tests
{
    public class MembershipTests : IClassFixture<DependencySetupFixture>
    {
        private ServiceProvider _serviceProvider;

        public MembershipTests(DependencySetupFixture fixture)
        {
            _serviceProvider = fixture.ServiceProvider;
        }

        [Fact]
        public async void CreateMembership_Add_New()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
            
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var MembershipService = scope.ServiceProvider.GetService<IMembershipService>();
                var mapper = scope.ServiceProvider.GetService<IMapper>();
                MembershipModel model = TestDataGenerator.GetMembershipModel();
                var mem = mapper.Map<Membership>(model);

                var Membership = await MembershipService.CreateMembership(model);
                Assert.True(Membership.MembershipId > 0, "Membership Created.");
            }

        }


        [Fact]
        public async void Membership_UpdateNextBillDate_Update()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
            Membership membership = new Membership();
            Person newBillablePerson = new Person();
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var organizationService = scope.ServiceProvider.GetService<IOrganizationService>();
                OrganizationModel organization = TestDataGenerator.GetOrganizationModel();
                var newOrganization = await organizationService.CreateOrganization(organization);

                var PersonService = scope.ServiceProvider.GetService<IPersonService>();
                PersonModel Person = TestDataGenerator.GetPersonModel();
                newBillablePerson = await PersonService.CreatePerson(Person);

                var MembershipService = scope.ServiceProvider.GetService<IMembershipService>();
                MembershipModel model = TestDataGenerator.GetMembershipModel();
                membership = await MembershipService.CreateMembership(model);

            }
            //Create a new context
            using (var scope = serviceScopeFactory.CreateScope())
            {

                var PersonService = scope.ServiceProvider.GetService<IPersonService>();
                var mapper_person = scope.ServiceProvider.GetService<IMapper>();
                var newPersonModel = mapper_person.Map<PersonModel>(newBillablePerson);

                var MembershipService = scope.ServiceProvider.GetService<IMembershipService>();
                var mapper_membership = scope.ServiceProvider.GetService<IMapper>();
                var newMembershipModel = mapper_membership.Map<MembershipModel>(membership);

                var updatenextbilldate = await MembershipService.UpdateNextBillDate(newMembershipModel.MembershipId);
                var updatedrecord = await MembershipService.GetMembershipById(newMembershipModel.MembershipId);

                Assert.True(updatedrecord.NextBillDate == DateTime.Today.AddMonths(1), "Next bill date updated.");

            }

        }

        [Fact]
        public async void Membership_UpdateMembership_Update()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
            Membership membership = new Membership();
            Person newBillablePerson = new Person();
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var organizationService = scope.ServiceProvider.GetService<IOrganizationService>();
                OrganizationModel organization = TestDataGenerator.GetOrganizationModel();
                var newOrganization = await organizationService.CreateOrganization(organization);

                var PersonService = scope.ServiceProvider.GetService<IPersonService>();
                PersonModel Person = TestDataGenerator.GetPersonModel();
                newBillablePerson = await PersonService.CreatePerson(Person);

                var MembershipService = scope.ServiceProvider.GetService<IMembershipService>();
                MembershipModel model = TestDataGenerator.GetMembershipModel();
                membership = await MembershipService.CreateMembership(model);

            }
            //Create a new context

            using (var scope = serviceScopeFactory.CreateScope())
            {

                var PersonService = scope.ServiceProvider.GetService<IPersonService>();
                var mapper_person = scope.ServiceProvider.GetService<IMapper>();
                var newPersonModel = mapper_person.Map<PersonModel>(newBillablePerson);

                var MembershipService = scope.ServiceProvider.GetService<IMembershipService>();
                var mapper_membership = scope.ServiceProvider.GetService<IMapper>();
                var newMembershipModel = mapper_membership.Map<MembershipModel>(membership);

                newMembershipModel.StartDate = DateTime.Today.AddDays(7);


                var updatedrecord = await MembershipService.UpdateMembership(newMembershipModel);

                Assert.True(updatedrecord.StartDate == DateTime.Today.AddDays(7), "Membership updated.");

            }

        }

        [Fact]
        public async void Membership_Get_MembershipEndDate()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {

                var membershipPeriodService = scope.ServiceProvider.GetService<IMembershipPeriodService>();
                MembershipPeriodModel periodmodel = TestDataGenerator.GetMembershipPeriodModel();
                var period = await membershipPeriodService.CreateMembershipPeriod(periodmodel);

                var MembershipService = scope.ServiceProvider.GetService<IMembershipService>();
                MembershipModel model = TestDataGenerator.GetMembershipModel();
                var Membership1 = await MembershipService.CreateMembership(model);

                var membershipEndDate = await MembershipService.GetMembershipEndDate(period.MembershipPeriodId, Membership1.StartDate);

                Assert.True(membershipEndDate == Membership1.StartDate.AddDays(period.Duration - 1), "Membership returns selected Date.");
            }

        }

        [Fact]
        public async void Membership_Get_By_AllMembershipDueByThroughDate()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var organizationService = scope.ServiceProvider.GetService<IOrganizationService>();
                OrganizationModel organization = TestDataGenerator.GetOrganizationModel();
                var newOrganization = await organizationService.CreateOrganization(organization);
                var PersonService = scope.ServiceProvider.GetService<IPersonService>();
                PersonModel Person = TestDataGenerator.GetPersonModel();
                var newPerson = await PersonService.CreatePerson(Person);

                var MembershipService = scope.ServiceProvider.GetService<IMembershipService>();
                MembershipModel model = TestDataGenerator.GetMembershipModel();
                var Membership1 = await MembershipService.CreateMembership(model);

                var memberships = await MembershipService.GetAllMembershipDueByThroughDate(Membership1.AutoPayEnabled, Membership1.NextBillDate.AddDays(1));

                Assert.True(!Extenstions.IsNullOrEmpty(memberships), "Membership has records.");
            }

        }


        [Fact]
        public async void Membership_Get_All()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var MembershipService = scope.ServiceProvider.GetService<IMembershipService>();

                MembershipModel model = TestDataGenerator.GetMembershipModel();

                await MembershipService.CreateMembership(model);

                //Add another

                model = TestDataGenerator.GetMembershipModel();

                await MembershipService.CreateMembership(model);

                var Memberships = await MembershipService.GetAllMemberships();

                Assert.True(!Extenstions.IsNullOrEmpty(Memberships), "Membership has records.");
            }

        }

        [Fact]
        public async void Membership_Get_By_Id()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var organizationService = scope.ServiceProvider.GetService<IOrganizationService>();
                OrganizationModel organization = TestDataGenerator.GetOrganizationModel();
                var newOrganization = await organizationService.CreateOrganization(organization);

                var PersonService = scope.ServiceProvider.GetService<IPersonService>();
                PersonModel Person = TestDataGenerator.GetPersonModel();
                var newPerson = await PersonService.CreatePerson(Person);


                var MembershipService = scope.ServiceProvider.GetService<IMembershipService>();

                MembershipModel model = TestDataGenerator.GetMembershipModel();

                var Membership = await MembershipService.CreateMembership(model);

                var newMembership = await MembershipService.GetMembershipById(Membership.MembershipId);

                Assert.True(newMembership.MembershipId == Membership.MembershipId, "Membership returns selected Id.");
            }

        }

    }
}