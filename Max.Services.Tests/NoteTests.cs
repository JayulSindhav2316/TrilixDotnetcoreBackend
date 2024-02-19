using AutoMapper;
using Max.Core;
using Max.Core.Models;
using Max.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;

namespace Max.Services.Tests
{
    public class NoteTests : IClassFixture<DependencySetupFixture>
    {
        private ServiceProvider _serviceProvider;

        public NoteTests(DependencySetupFixture fixture)
        {
            _serviceProvider = fixture.ServiceProvider;
        }

        [Fact]
        public async void CreateNote_Add_New()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var NoteService = scope.ServiceProvider.GetService<INoteservice>();

                NotesModel model = TestDataGenerator.GetNotesModel();

                var Note = await NoteService.CreateNote(model);

                Assert.True(Note.NoteId > 0, "Note Created.");
            }
            
        }

        [Fact]
        public async void CreateNote_Validate_Validate()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var NoteService = scope.ServiceProvider.GetService<INoteservice>();
                NotesModel model = TestDataGenerator.GetNotesModel();
                model.Notes = null;                

                var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => NoteService.CreateNote(model));

                Assert.IsType<InvalidOperationException>(ex);
                Assert.Contains("Note can not be empty.", ex.Message);
            }

        }

        [Fact]
        public async void Notes_Get_GetAll()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var NoteService = scope.ServiceProvider.GetService<INoteservice>();

                NotesModel model = TestDataGenerator.GetNotesModel();

                await NoteService.CreateNote(model);

                //Add another

                model = TestDataGenerator.GetNotesModel();

                await NoteService.CreateNote(model);

                var Notes = await NoteService.GetAllNotes();

                Assert.True(!Extenstions.IsNullOrEmpty(Notes), "Note has records.");
            }

        }

        [Fact]
        public async void Note_Get_GetBy_PersonId()
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

                var NoteService = scope.ServiceProvider.GetService<INoteservice>();
                NotesModel model = TestDataGenerator.GetNotesModel();
                model.EntityId = newPerson.EntityId??0;
                var Notes = await NoteService.CreateNote(model);

                var returnedNote = await NoteService.GetNotesByEntityId(newPerson.EntityId ?? 0);

                Assert.True(!Extenstions.IsNullOrEmpty(returnedNote), "Note has records.");
            }

        }

        [Fact]
        public async void Note_Update_Updatenote()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var NoteService = scope.ServiceProvider.GetService<INoteservice>();
                NotesModel model = TestDataGenerator.GetNotesModel();
                var Notes = await NoteService.CreateNote(model);
                //var returnedNote = await NoteService.GetNoteById(Notes.NoteId);

                var mapper = scope.ServiceProvider.GetService<IMapper>();
                var newNoteModel = mapper.Map<NotesModel>(Notes);

                newNoteModel.Notes = "Changed Notes";

                var updatednote = await NoteService.UpdateNote(newNoteModel);

                Assert.True(updatednote.Notes == "Changed Notes", "Note updated.");
            }

        }

        [Fact]
        public async void Note_Delete_Deletenote()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var NoteService = scope.ServiceProvider.GetService<INoteservice>();
                NotesModel model = TestDataGenerator.GetNotesModel();
                var Notes = await NoteService.CreateNote(model);

                await NoteService.DeleteNote(Notes.NoteId);

                var deleteddnote = await NoteService.GetNoteById(Notes.NoteId);                

                Assert.True(deleteddnote == null, "Note Deleted.");
            }

        }

        [Fact]
        public async void Note_Get_GetById()
        {
            var serviceScopeFactory = _serviceProvider.GetRequiredService<IServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var NoteService = scope.ServiceProvider.GetService<INoteservice>();

                NotesModel model = TestDataGenerator.GetNotesModel();

                var Notes = await NoteService.CreateNote(model);

                var returnedNote = await NoteService.GetNoteById(Notes.NoteId);

                Assert.True(returnedNote.NoteId == Notes.NoteId, "Note returns selected Id.");
            }

        }

        


    }
}