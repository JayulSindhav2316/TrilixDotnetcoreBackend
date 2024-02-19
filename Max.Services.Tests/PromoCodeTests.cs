using Xunit;
using Max.Core.Models;
using Max.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Max.Core;
using System;

namespace Max.Services.Tests
{
    public class PromoCodeTests : IClassFixture<DependencySetupFixture>
    {
        private ServiceProvider _serviceProvider;

        public PromoCodeTests(DependencySetupFixture fixture)
        {
            _serviceProvider = fixture.ServiceProvider;
        }

        [Fact]
        public async void CreatePromoCode_Add_New()

        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var promocodeService = scope.ServiceProvider.GetService<IPromoCodeService>();
                PromoCodeModel promocode = TestDataGenerator.GetPromoCodeModel();
                var newPromocode = await promocodeService.CreatePromoCode(promocode);

                Assert.True(newPromocode.PromoCodeId > 0, "Promocode Created.");
                
            } 

        }
        [Fact]
        public async void PromoCode_GetById()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var promocodeService = scope.ServiceProvider.GetService<IPromoCodeService>();
                PromoCodeModel promocode = TestDataGenerator.GetPromoCodeModel();
                var newPromoCode = await promocodeService.CreatePromoCode(promocode);

             var selectedpromocode= await promocodeService.GetPromoCodeById(newPromoCode.PromoCodeId);                

             Assert.True(selectedpromocode.PromoCodeId == newPromoCode.PromoCodeId, "PromoCode returns selected Id.");
               

            }

        }

        [Fact]
        public async void UpdatePromoCode()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var promocodeService = scope.ServiceProvider.GetService<IPromoCodeService>();
                PromoCodeModel promocode = TestDataGenerator.GetPromoCodeModel();
                var newPromocode = await promocodeService.CreatePromoCode(promocode);

                promocode.PromoCodeId = newPromocode.PromoCodeId;

                //Change Code,Description,GL account
                promocode.Code = "Changed promocode";
                promocode.Description = "Changed Description";
                promocode.GlAccountId = 99;

                await promocodeService.UpdatePromoCode(promocode);

                var updatedPromoCode = await promocodeService.GetPromoCodeById(newPromocode.PromoCodeId);
                
                Assert.True(updatedPromoCode.Code == newPromocode.Code, "Code Updated.");
                Assert.True(updatedPromoCode.Description == "Changed Description", "PromoCode Description Updated.");
                Assert.True(updatedPromoCode.GlAccountId == 99, "GL Account Updated.");

            }

        }

        [Fact]
        public async void DeletePromoCode()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var promocodeService = scope.ServiceProvider.GetService<IPromoCodeService>();
                PromoCodeModel promocode = TestDataGenerator.GetPromoCodeModel();
                var newPromocode = await promocodeService.CreatePromoCode(promocode);          
                                
                await promocodeService.DeletePromoCode(newPromocode.PromoCodeId);

                var DeletedPromoCode = await promocodeService.GetPromoCodeById(newPromocode.PromoCodeId);

                Assert.True(DeletedPromoCode == null, "PromoCode Deleted.");

            }

        }

        [Fact]
        public async void GeneratePromoCode()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var promocodeService = scope.ServiceProvider.GetService<IPromoCodeService>();
                PromoCodeModel promocode = TestDataGenerator.GetPromoCodeModel();
                var newPromocode = await promocodeService.GenratePromoCode();          
                                
                Assert.True(newPromocode.Code.Length > 0, "PromoCode Generated.");

            }

        }

        [Theory]
        [InlineData("mindatecheck")]
        [InlineData("maxdatecheck")]
        public async void PromoCode_Validate_ValidateStartDate(string param)
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var promocodeService = scope.ServiceProvider.GetService<IPromoCodeService>();
                PromoCodeModel promocode = TestDataGenerator.GetPromoCodeModel();
                if (param == "maxdatecheck")
                { 
                    promocode.StartDate = DateTime.Today.AddDays(-1);
                    var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => promocodeService.CreatePromoCode(promocode));
                    Assert.Contains("Start date cant be a past date.", ex.Message);
                }
                else // mindatecheck
                {
                    promocode.StartDate = promocode.ExpirationDate.Value.AddDays(1);
                    var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => promocodeService.CreatePromoCode(promocode));
                    Assert.Contains("Start date cant be greater than expiration date.", ex.Message);
                }

            

        }
    }


    [Theory]
        [InlineData(null, "promocodedescription")] // null code
        [InlineData("", "promocodedescription")]   // Blank Membershiptype name   
        [InlineData("promo_code", null)] // null Membershiptype code
        [InlineData("promo_code", "")] // Blank Membershiptype code  
        [InlineData("promo_code", "promocodedescription")] // Blank Membershiptype code  
        public async void PromoCode_Validate_Validate(string promocode, string description)
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var promocodeService = scope.ServiceProvider.GetService<IPromoCodeService>();
                PromoCodeModel promocodemodel = TestDataGenerator.GetPromoCodeModel();

                promocodemodel.Code = promocode;
                promocodemodel.Description = description;

                if (description == "" || description == null)
                {
                    var ex = await Assert.ThrowsAsync<NullReferenceException>(() => promocodeService.CreatePromoCode(promocodemodel));
                    Assert.Contains("Promo Code Description can not be empty.", ex.Message);
                }
                else if (promocode == "" || promocode == null)
                {
                    var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => promocodeService.CreatePromoCode(promocodemodel));
                    Assert.Contains("Promo Code can not be empty.", ex.Message);
                }
                else
                {
                    var newPromocode = await promocodeService.CreatePromoCode(promocodemodel);
                    var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => promocodeService.CreatePromoCode(promocodemodel));
                    Assert.Contains("Promo Code already exists.", ex.Message);
                }

            }

        }


        [Fact]
        public async void GeteAllPromoCodes()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var promocodeService = scope.ServiceProvider.GetService<IPromoCodeService>();
                PromoCodeModel promocode = TestDataGenerator.GetPromoCodeModel();
                await promocodeService.CreatePromoCode(promocode);

                //Add another
                promocode = TestDataGenerator.GetPromoCodeModel();
                await promocodeService.CreatePromoCode(promocode);

                var allPromoCodes = await promocodeService.GetAllPromoCodes();

                Assert.True(!Extenstions.IsNullOrEmpty(allPromoCodes), "PromoCodes has records.");               

            }

        }

    }
}
