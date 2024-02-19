using AutoMapper;
using Max.Core;
using Max.Core.Models;
using Max.Data.DataModel;
using Max.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using Xunit;

namespace Max.Services.Tests
{
    public class LookupTests : IClassFixture<DependencySetupFixture>
    {
        private ServiceProvider _serviceProvider;

        public LookupTests(DependencySetupFixture fixture)
        {
            _serviceProvider = fixture.ServiceProvider;
        }

        [Fact]
        public async void CreateLookup_Create_AddNew()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {                
                var LookupService = scope.ServiceProvider.GetService<ILookupService>();
                LookupModel Lookupmodel = TestDataGenerator.GetLookupModel();
                var lookup = await LookupService.CreateLookup(Lookupmodel);


                Assert.True(lookup.LookupId > 0, "Lookup record Created.");
            }

        }

        [Fact]
        public async void GetLookup_Get_ById()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var LookupService = scope.ServiceProvider.GetService<ILookupService>();
                LookupModel Lookupmodel = TestDataGenerator.GetLookupModel();
                var lookup = await LookupService.CreateLookup(Lookupmodel);

                var selectedrecord= await LookupService.GetLookupById(lookup.LookupId);

                Assert.True(selectedrecord.LookupId==lookup.LookupId, "Returns selected Lookup record.");
            }

        }

        [Fact]
        public async void GetLookup_Get_All()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var LookupService = scope.ServiceProvider.GetService<ILookupService>();
                LookupModel Lookupmodel = TestDataGenerator.GetLookupModel();
                var lookup1 = await LookupService.CreateLookup(Lookupmodel);

                //add another
                Lookupmodel = TestDataGenerator.GetLookupModel();
                var lookup2 = await LookupService.CreateLookup(Lookupmodel);

                var selectedrecords = await LookupService.GetAllLookups();

                Assert.True(selectedrecords.Count() > 1 , "Returns All Lookup records.");
            }

        }

        [Fact]
        public async void GetLookup_Get_LookupValueByGroupName()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var LookupService = scope.ServiceProvider.GetService<ILookupService>();
                LookupModel Lookupmodel = TestDataGenerator.GetLookupModel();
                var lookup = await LookupService.CreateLookup(Lookupmodel);                

                var selectedrecords = await LookupService.GetLookupValueByGroupName(lookup.Group);

                Assert.True(selectedrecords == lookup.Values , "Returns All Lookup records.");
            }

        }

        [Fact]
        public async void Lookup_Delete_Delete()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var LookupService = scope.ServiceProvider.GetService<ILookupService>();
                LookupModel Lookupmodel = TestDataGenerator.GetLookupModel();
                var lookup = await LookupService.CreateLookup(Lookupmodel);

                await LookupService.DeleteLookup(lookup.LookupId);

                var deletedrecord = await LookupService.GetLookupById(lookup.LookupId);

                Assert.True(deletedrecord == null, "Returns All Lookup records.");
            }

        }

        [Fact]
        public async void Lookup_Update_Update()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
            Lookup lookup = new Lookup();
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var LookupService = scope.ServiceProvider.GetService<ILookupService>();
                LookupModel Lookupmodel = TestDataGenerator.GetLookupModel();
                lookup = await LookupService.CreateLookup(Lookupmodel);                           

                var mapper = scope.ServiceProvider.GetService<IMapper>();
                var newlookup = mapper.Map<LookupModel>(lookup);
                newlookup.Group = "Changed Name";

                await LookupService.UpdateLookup(newlookup);

                var updatedrecord = await LookupService.GetLookupById(newlookup.LookupId);

                Assert.True(updatedrecord.Group == "Changed Name", "Lookup record Updated.");
            }

        }

        [Theory]
        [InlineData(null)] // null Group 
        [InlineData("")]   // Blank Group  
        public async void Lookup_Validate_Validate(string groupname)
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
            Lookup lookup = new Lookup();
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var LookupService = scope.ServiceProvider.GetService<ILookupService>();
                LookupModel Lookupmodel = TestDataGenerator.GetLookupModel();

                Lookupmodel.Group = groupname;

               if (groupname == null)
                {
                    var ex = await Assert.ThrowsAsync<NullReferenceException>(() => LookupService.CreateLookup(Lookupmodel));
                    Assert.Contains("Lookup Name can not be NULL.", ex.Message);
                }
                else // for Blank group
                {                   
                    var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => LookupService.CreateLookup(Lookupmodel));
                    Assert.Contains("Lookup Name can not be empty.", ex.Message);
                }
                
            }

        }

    }
}