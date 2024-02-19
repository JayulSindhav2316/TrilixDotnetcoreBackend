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
    public class EmailTests : IClassFixture<DependencySetupFixture>
    {
        private ServiceProvider _serviceProvider;

        public EmailTests(DependencySetupFixture fixture)
        {
            _serviceProvider = fixture.ServiceProvider;
        }

        //[Fact]
        //public async void Email_Send_HtmlInvoice()
        //{
        //    var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

        //    using (var scope = serviceScopeFactory.CreateScope())
        //    {              

        //        var emailService = scope.ServiceProvider.GetService<IEmailService>();

        //        EmailMessageModel Emailmessagemodel = TestDataGenerator.GetEmailMessageModel();

        //        var sentmail = await emailService.SendHtmlInvoice(Emailmessagemodel);

        //        Assert.True(sentmail == true , "Mail has been sent.");
        //    }

        //}

        [Fact]
        public async void Email_Send_HtmlReceipt()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {

                var emailService = scope.ServiceProvider.GetService<IEmailService>();

                EmailMessageModel Emailmessagemodel = TestDataGenerator.GetEmailMessageModel();

                var sentmail = await emailService.SendHtmlReceipt(Emailmessagemodel);

                Assert.True(sentmail == true, "Mail has been sent.");
            }

        }

        [Fact(Skip = "host environment variable to get root directory path could not be fetched")]
        public async void Email_Send_AutoBillingNotification()
        {
            // Pending test case
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())            {
               
                var emailService = scope.ServiceProvider.GetService<IEmailService>();

                AutoBillingEmailNotificationModel Emailnoticicationmodel = TestDataGenerator.GetAutobillingemailnoticicationModel();

                var sentmail = await emailService.SendAutoBillingNotification(Emailnoticicationmodel);

                Assert.True(sentmail == true, "Mail has been sent.");
            }

        }

        [Fact(Skip = "host environment variable to get root directory path could not be fetched")]
        public async void Email_Send_MultiFactorNotification()
        {
            // Pending test case
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {

                var emailService = scope.ServiceProvider.GetService<IEmailService>();

                //AutoBillingEmailNotificationModel Emailmessagemodel = TestDataGenerator.GetAutobillingemailnoticicationModel();

                var sentmail = await emailService.SendMultiFactorNotification("membermax","rohitt@anittechnologies.com", "AB12CD") ;

                Assert.True(sentmail == true, "Mail has been sent.");
            }

        }      


    }
}