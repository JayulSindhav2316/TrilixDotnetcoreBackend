using Xunit;
using Max.Core.Models;
using Max.Core;
using Max.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using AutoMapper;
using System;
using System.Collections.Generic;
using Xunit.Priority;

namespace Max.Services.Tests
{
    [TestCaseOrderer(PriorityOrderer.Name, PriorityOrderer.Assembly)]
    public class InvoiceTests : IClassFixture<DependencySetupFixture>
    {
        private ServiceProvider _serviceProvider;

        public InvoiceTests(DependencySetupFixture fixture)
        {
            _serviceProvider = fixture.ServiceProvider;
        }

        [Fact]
        public async void CreateInvoice_Add_New()
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

                var InvoiceService = scope.ServiceProvider.GetService<IInvoiceService>();

                InvoiceModel model = TestDataGenerator.GetInvoiceModel();

                var invoice = await InvoiceService.CreateInvoice(model);
                Assert.True(invoice.InvoiceId > 0, "Invoice Created.");
            }

        }      


        [Fact, Priority(-1)]
        public async void CreateInvoice_Validate_Validate_Nobillablepersonrecord()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {                             

                var InvoiceService = scope.ServiceProvider.GetService<IInvoiceService>();
                InvoiceModel model = TestDataGenerator.GetInvoiceModel();                
                var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => InvoiceService.CreateInvoice(model));

                Assert.IsType<KeyNotFoundException>(ex);
                Assert.Contains("No entity found with ID: " + model.BillableEntityId + ".", ex.Message);
            }

        }

        [Theory]
        [InlineData(null, 1)] // null personId        
        [InlineData(1, null)] // null BillablepersonId      
        public async void CreateInvoice_Validate_Validate_NullpersonIds(int personId,int billablepersonid)
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {

                var InvoiceService = scope.ServiceProvider.GetService<IInvoiceService>();
                InvoiceModel model = TestDataGenerator.GetInvoiceValidationModel(personId,billablepersonid);

                

                var param = personId==0 ? "Entity Id" : "Billable Member";

                if (personId == 0)
                {
                    var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => InvoiceService.CreateInvoice(model));
                    Assert.Contains(param + " not defined.", ex.Message);
                }
                else
                {
                    var ex = await Assert.ThrowsAsync<NullReferenceException>(() => InvoiceService.CreateInvoice(model));
                    Assert.Contains(param + " not defined.", ex.Message);
                }
                
            }

        }

        [Fact]
        public async void Invoice_Get_All()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var OrganizationService = scope.ServiceProvider.GetService<IOrganizationService>();
                OrganizationModel organizationmodel = TestDataGenerator.GetOrganizationModel();
                await OrganizationService.CreateOrganization(organizationmodel);

                var PersonService = scope.ServiceProvider.GetService<IPersonService>();
                PersonModel personmodel = TestDataGenerator.GetPersonModel();
                await PersonService.CreatePerson(personmodel);

                var InvoiceService = scope.ServiceProvider.GetService<IInvoiceService>();
                InvoiceModel Invoicemodel = TestDataGenerator.GetInvoiceModel();
                await InvoiceService.CreateInvoice(Invoicemodel);

                //Add another

                Invoicemodel = TestDataGenerator.GetInvoiceModel();

                await InvoiceService.CreateInvoice(Invoicemodel);

                var invoices = await InvoiceService.GetAllInvoices();

                Assert.True(!Extenstions.IsNullOrEmpty(invoices), "Invoice has records.");
            }

        }

        [Fact]
        public async void Invoice_Get_By_Id()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var InvoiceService = scope.ServiceProvider.GetService<IInvoiceService>();

                InvoiceModel model = TestDataGenerator.GetInvoiceModel();

                var invoice = await InvoiceService.CreateInvoice(model);

                var newInvoice = await InvoiceService.GetInvoiceById(invoice.InvoiceId);

                Assert.True(newInvoice.InvoiceId == invoice.InvoiceId, "Invoice returns selected Id.");
            }

        }

        [Fact]
        public async void Invoice_Delte_Delete()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
            InvoiceModel invoice = new InvoiceModel();
            using (var scope = serviceScopeFactory.CreateScope())
            {

                var organizationService = scope.ServiceProvider.GetService<IOrganizationService>();
                OrganizationModel organization = TestDataGenerator.GetOrganizationModel();
                var newOrganization = await organizationService.CreateOrganization(organization);

                var PersonService = scope.ServiceProvider.GetService<IPersonService>();
                PersonModel Person = TestDataGenerator.GetPersonModel();
                var newPerson = await PersonService.CreatePerson(Person);

                var InvoiceService = scope.ServiceProvider.GetService<IInvoiceService>();
                InvoiceModel model = TestDataGenerator.GetInvoiceModel();
                invoice = await InvoiceService.CreateInvoice(model);
            }
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var InvoiceService = scope.ServiceProvider.GetService<IInvoiceService>();
                var mapper = scope.ServiceProvider.GetService<IMapper>();
                var newInvoiceModel = mapper.Map<InvoiceModel>(invoice);

                await InvoiceService.DeleteInvoice(invoice.InvoiceId);                

                var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => InvoiceService.GetInvoiceById(invoice.InvoiceId));

                Assert.IsType<KeyNotFoundException>(ex);
                Assert.Contains("Invoice: " + invoice.InvoiceId + " not found.", ex.Message);                
            }

        }

        [Fact]
        public async void Invoice_Delte_DeletePending()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
            InvoiceModel invoice = new InvoiceModel();
            using (var scope = serviceScopeFactory.CreateScope())
            {

                var organizationService = scope.ServiceProvider.GetService<IOrganizationService>();
                OrganizationModel organization = TestDataGenerator.GetOrganizationModel();
                var newOrganization = await organizationService.CreateOrganization(organization);

                var PersonService = scope.ServiceProvider.GetService<IPersonService>();
                PersonModel Person = TestDataGenerator.GetPersonModel();
                var newPerson = await PersonService.CreatePerson(Person);

                var InvoiceService = scope.ServiceProvider.GetService<IInvoiceService>();
                InvoiceModel model = TestDataGenerator.GetInvoiceModel();
                model.Status = (int)InvoiceStatus.Draft; // change status to Draft
                model.InvoiceType = "Membership"; // change invoice typy
                await InvoiceService.CreateInvoice(model);

                //Add another

                model = TestDataGenerator.GetInvoiceModel();
                await InvoiceService.CreateInvoice(model);

                var selectedduesinvoices= await InvoiceService.GetAllInvoices();

                await InvoiceService.DeletePendingInvoices("Membership");

               selectedduesinvoices = await InvoiceService.GetAllInvoices();

                Assert.False(selectedduesinvoices.Where(x => x.InvoiceType == "Membership" && x.Status== (int)InvoiceStatus.Draft).Count()==1, "Pending invoice has been deleted");

            }            

        }

        [Fact]
        public async void Invoice_Get_By_BillableEntityId()
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

                var InvoiceService = scope.ServiceProvider.GetService<IInvoiceService>();

                InvoiceModel model = TestDataGenerator.GetInvoiceModel();
                model.EntityId = (int)newPerson.EntityId;
                var invoice = await InvoiceService.CreateInvoice(model);

                var newInvoice = await InvoiceService.GetAllInvoicesByEntityId(invoice.BillableEntityId??0,"","");
                var billableEntityId = invoice.BillableEntityId;               
                Assert.True(billableEntityId == 1, "Invoice returns selected Id.");

            }

        }

        [Fact]
        public async void Getinvoice_Get_InvoicedetailId_ByInvoiceId()
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

                var InvoiceService = scope.ServiceProvider.GetService<IInvoiceService>();
                InvoiceModel model = TestDataGenerator.GetInvoiceModel();
                var invoice = await InvoiceService.CreateInvoice(model);

                var selectedrecord = await InvoiceService.GetInvoiceDetailsByInvoiceId(invoice.InvoiceId);

                Assert.True(selectedrecord.InvoiceId == invoice.InvoiceId, "returns selected id");
            }

        }

    }
}