using System;
using Xunit;
using Max.Core.Models;
using Max.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using Max.Data.DataModel;
using AutoMapper;

namespace Max.Services.Tests
{
    public class PersonTests : IClassFixture<DependencySetupFixture>
    {
        private ServiceProvider _serviceProvider;

        public PersonTests(DependencySetupFixture fixture)
        {
            _serviceProvider = fixture.ServiceProvider;
        }

        [Fact]
        public async void CreatePerson_Add_New()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var PersonService = scope.ServiceProvider.GetService<IPersonService>();

                PersonModel Person = TestDataGenerator.GetPersonModel();

                var newPerson = await PersonService.CreatePerson(Person);
                Assert.True(newPerson.PersonId > 0, "Person Created.");
            }

        }

        [Fact]
        public async void GetPerson_Get_ById()
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

                var selectedrecord = await PersonService.GetPersonById(newPerson.PersonId);

                Assert.True(selectedrecord.PersonId == newPerson.PersonId, "Department returns selected Id.");
            }

        }

        [Fact]
        public async void GetPerson_Get_PersonProfile_ById()
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

                var selectedrecord = await PersonService.GetPersonById(newPerson.PersonId);

                Assert.True(selectedrecord.PersonId == newPerson.PersonId, "Person returns selected Id.");
            }

        }



        [Fact]
        public async void UpdatePerson_Update()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
            Person newPerson = new Person();
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var organizationService = scope.ServiceProvider.GetService<IOrganizationService>();
                OrganizationModel organization = TestDataGenerator.GetOrganizationModel();
                var newOrganization = await organizationService.CreateOrganization(organization);
                var PersonService = scope.ServiceProvider.GetService<IPersonService>();
                PersonModel Person = TestDataGenerator.GetPersonModel();
                newPerson = await PersonService.CreatePerson(Person);
            }
            //Create a new context
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var PersonService = scope.ServiceProvider.GetService<IPersonService>();
                var mapper = scope.ServiceProvider.GetService<IMapper>();
                var newPersonModel = mapper.Map<PersonModel>(newPerson);
                newPersonModel.LastName = "Changed Name";
                await PersonService.UpdatePerson(newPersonModel);
                var updatedPerson = await PersonService.GetPersonById(newPerson.PersonId);
                Assert.True(updatedPerson.LastName == "Changed Name", "Person Updated.");
            }
        }

        [Fact]
        public async void UpdatePerson_Update_Email()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
            Person newPerson = new Person();
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var organizationService = scope.ServiceProvider.GetService<IOrganizationService>();
                OrganizationModel organization = TestDataGenerator.GetOrganizationModel();
                var newOrganization = await organizationService.CreateOrganization(organization);
                var PersonService = scope.ServiceProvider.GetService<IPersonService>();
                PersonModel Person = TestDataGenerator.GetPersonModel();
                newPerson = await PersonService.CreatePerson(Person);
            }
            //Create a new context
           
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var PersonService = scope.ServiceProvider.GetService<IPersonService>();
                var mapper = scope.ServiceProvider.GetService<IMapper>();
                var newPersonModel = mapper.Map<PersonModel>(newPerson);                
                var emails = newPersonModel.Emails.ToList();

                foreach (var item in emails)
                {
                    item.EmailAddress= "changedemail" + item.EmailAddressType + "@test.com";                   

                }

                await PersonService.UpdatePerson(newPersonModel);
                var updatedPerson = await PersonService.GetPersonById(newPersonModel.PersonId);               

                Assert.Contains(updatedPerson.Emails,p=>p.EmailAddress.Contains("changedemail")==true);
            }

        }

        [Fact]
        public async void UpdatePerson_Update_Phone()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();
            Person newPerson = new Person();
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var organizationService = scope.ServiceProvider.GetService<IOrganizationService>();
                OrganizationModel organization = TestDataGenerator.GetOrganizationModel();
                var newOrganization = await organizationService.CreateOrganization(organization);
                var PersonService = scope.ServiceProvider.GetService<IPersonService>();
                PersonModel Person = TestDataGenerator.GetPersonModel();
                newPerson = await PersonService.CreatePerson(Person);
            }

            //Create a new context
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var PersonService = scope.ServiceProvider.GetService<IPersonService>();
                var mapper = scope.ServiceProvider.GetService<IMapper>();
                var newPersonModel = mapper.Map<PersonModel>(newPerson);                
                var phonenos = newPersonModel.Phones.ToList();

                foreach (var item in phonenos)
                {
                    item.PhoneNumber = "1233219999";
                }

                await PersonService.UpdatePerson(newPersonModel);
                var updatedPerson = await PersonService.GetPersonById(newPersonModel.PersonId);

                Assert.Contains(updatedPerson.Phones, p => p.PhoneNumber.Contains("1233219999") == true);
            }

        }

        [Fact]
        public async void DeletePerson_Delete()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {

                var PersonService = scope.ServiceProvider.GetService<IPersonService>();

                PersonModel Person = TestDataGenerator.GetPersonModel();

                var newPerson = await PersonService.CreatePerson(Person);

                await PersonService.DeletePerson(newPerson.PersonId);

                var deletedPerson = await PersonService.GetPersonById(newPerson.PersonId);

                Assert.True(deletedPerson.PersonId==0, "Person Deleted.");

            }

        }

        [Fact]
        public async void SearcchPerson_By_EmailAddress()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {

                var PersonService = scope.ServiceProvider.GetService<IPersonService>();

                PersonModel Person = TestDataGenerator.GetPersonModel();

                var newPerson = await PersonService.CreatePerson(Person);

                var searchEmail = newPerson.Emails.OfType<Data.DataModel.Email>().FirstOrDefault();

                //Add Another  Person
                Person = TestDataGenerator.GetPersonModel();

                newPerson = await PersonService.CreatePerson(Person);

                //Search  Person By Email

                var  person  = await PersonService.GetAllPersonsByEmail(searchEmail.EmailAddress,"",0);

                var result = person.Where(x => x.Emails.Any(y => y.EmailAddress == searchEmail.EmailAddress)).FirstOrDefault();

                Assert.True(result.PersonId >  0, "Person Found by EmailId.");

            }

        }
        [Fact]
        public async void SearcchPerson_By_PhoneNumber()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {

                var PersonService = scope.ServiceProvider.GetService<IPersonService>();

                PersonModel Person = TestDataGenerator.GetPersonModel();

                var newPerson = await PersonService.CreatePerson(Person);

                var searchPhone = newPerson.Phones.OfType<Data.DataModel.Phone>().FirstOrDefault();

                //Add Another  Person
                Person = TestDataGenerator.GetPersonModel();

                newPerson = await PersonService.CreatePerson(Person);

                //Search  Person By Phone

                var person = await PersonService.GetAllPersonsByPhoneNumber(searchPhone.PhoneNumber,"",0);

                var result = person.Where(x => x.Phones.Any(y => y.PhoneNumber == searchPhone.PhoneNumber)).FirstOrDefault();

                Assert.True(result.PersonId > 0, "Person Found by Phone Number.");

            }

        }

        [Fact]
        public async void SearcchPerson_By_First_And_LastName()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {

                var PersonService = scope.ServiceProvider.GetService<IPersonService>();

                PersonModel Person = TestDataGenerator.GetPersonModel();

                var newPerson = await PersonService.CreatePerson(Person);

                var searchFirstName = newPerson.FirstName;
                var searchLastName = newPerson.LastName;

                //Add Another  Person
                Person = TestDataGenerator.GetPersonModel();

                newPerson = await PersonService.CreatePerson(Person);

                //Search  Person By Fisrt & Last Name

                var person = await PersonService.GetAllPersonsByFirstAndLastName(searchFirstName, searchLastName,"",0);

                var result = person.Where(x => x.FirstName== searchFirstName  &&  x.LastName== searchLastName).FirstOrDefault();

                Assert.True(result.PersonId > 0, "Person Found by Fisrt & LastName.");

            }

        }
    }
}