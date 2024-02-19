using Xunit;
using Max.Core.Models;
using Max.Core;
using Max.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace Max.Services.Tests
{
    public class CommunicationTests : IClassFixture<DependencySetupFixture>
    {
        private ServiceProvider _serviceProvider;

        public CommunicationTests(DependencySetupFixture fixture)
        {
            _serviceProvider = fixture.ServiceProvider;
        }

        [Fact]
        public async void CreateCommunication_Add_New()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var CommunicationService = scope.ServiceProvider.GetService<ICommunicationService>();

                CommunicationModel model = TestDataGenerator.GetCommunicationModel();

                var commmunication = await CommunicationService.CreateCommunication(model);
                Assert.True(commmunication.CommunicationId > 0, "Communication Created.");
            }

        }

        [Fact]
        public async void Communication_Update()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var CommunicationService = scope.ServiceProvider.GetService<ICommunicationService>();
                CommunicationModel model = TestDataGenerator.GetCommunicationModel();
                var communication = await CommunicationService.CreateCommunication(model);

                model.CommunicationId = communication.CommunicationId;
                model.Notes = "Changed Notes";

                await CommunicationService.UpdateCommunication(model);

                Assert.True(communication.Notes == "Changed Notes", "Communication updated.");
            }

        }

        [Fact]
        public async void Communication_Delete()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var CommunicationService = scope.ServiceProvider.GetService<ICommunicationService>();
                CommunicationModel model = TestDataGenerator.GetCommunicationModel();
                var communication = await CommunicationService.CreateCommunication(model);

                await CommunicationService.DeleteCommunication(communication.CommunicationId);

                var deletedCommmunication = await CommunicationService.GetCommunicationById(communication.CommunicationId);

                Assert.True(deletedCommmunication == null, "Communication Deleted.");
                
            }

        }

        [Fact]
        public async void Communication_Get_All()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var CommunicationService = scope.ServiceProvider.GetService<ICommunicationService>();

                CommunicationModel model = TestDataGenerator.GetCommunicationModel();

                await CommunicationService.CreateCommunication(model);

                //Add another

                model = TestDataGenerator.GetCommunicationModel();

                await CommunicationService.CreateCommunication(model);

                var commmunication = await CommunicationService.GetAllCommunications();

                Assert.True(!Extenstions.IsNullOrEmpty(commmunication), "Communication has records.");
            }

        }

        [Fact]
        public async void Communication_Get_By_Id()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var CommunicationService = scope.ServiceProvider.GetService<ICommunicationService>();

                CommunicationModel model = TestDataGenerator.GetCommunicationModel();

                var communication = await CommunicationService.CreateCommunication(model);

                var newCommmunication = await CommunicationService.GetCommunicationById(communication.CommunicationId);

                Assert.True(newCommmunication.CommunicationId == communication.CommunicationId, "Communication returns selected Id.");
            }

        }

        [Fact]
        public async void Communication_Get_By_PersonId()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var CommunicationService = scope.ServiceProvider.GetService<ICommunicationService>();

                CommunicationModel model = TestDataGenerator.GetCommunicationModel();

                var communication = await CommunicationService.CreateCommunication(model);

                var newCommmunication = await CommunicationService.GetAllCommunicationsByEntityIdId(model.EntityId);

                Assert.True(newCommmunication.Where(x => x.EntityId== model.EntityId).Count() > 0, "Communication returns selected Id.");
            }

        }

    }
}