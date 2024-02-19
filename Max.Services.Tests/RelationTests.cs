using Xunit;
using Max.Core.Models;
using Max.Core;
using Max.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace Max.Services.Tests
{
    public class RelationTests : IClassFixture<DependencySetupFixture>
    {
        private ServiceProvider _serviceProvider;

        public RelationTests(DependencySetupFixture fixture)
        {
            _serviceProvider = fixture.ServiceProvider;
        }

        [Fact]
        public async void CreateRelation_Add_New()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var RelationService = scope.ServiceProvider.GetService<IRelationService>();

                RelationModel model = TestDataGenerator.GetRelationModel();

                var Relation = await RelationService.CreateRelation(model);
                Assert.True(Relation.RelationId > 0, "Relation Created.");
            }

        }

        [Fact]
        public async void Relation_Get_All()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var RelationService = scope.ServiceProvider.GetService<IRelationService>();

                RelationModel model = TestDataGenerator.GetRelationModel();

                await RelationService.CreateRelation(model);

                //Add another

                model = TestDataGenerator.GetRelationModel();

                await RelationService.CreateRelation(model);

                var Relations = await RelationService.GetAllRelations();

                Assert.True(!Extenstions.IsNullOrEmpty(Relations), "Relation has records.");
            }

        }

        [Fact]
        public async void Relation_Get_By_Id()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var RelationService = scope.ServiceProvider.GetService<IRelationService>();

                RelationModel model = TestDataGenerator.GetRelationModel();

                var Relation = await RelationService.CreateRelation(model);

                var newRelation = await RelationService.GetRelationById(Relation.RelationId);

                Assert.True(newRelation.RelationId == Relation.RelationId, "Relation returns selected Id.");
            }

        }

    }
}