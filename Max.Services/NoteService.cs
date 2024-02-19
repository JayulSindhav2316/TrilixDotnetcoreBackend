using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.Repositories;
using Max.Data.Interfaces;
using Max.Data.DataModel;
using Max.Services.Interfaces;
using Max.Core.Models;
using Max.Core;
using Max.Data;
using Max.Services;
namespace Max.Services
{
    public class NoteService : INoteservice
    {

        private readonly IUnitOfWork _unitOfWork;
        public NoteService(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Note>> GetAllNotes()
        {
            return await _unitOfWork.Notes
                .GetAllNotesAsync();
        }

        public async Task<Note> GetNoteById(int id)
        {
            return await _unitOfWork.Notes
                .GetNoteByIdAsync(id);
        }

        public async Task<IEnumerable<Note>> GetNotesByEntityId(int id)
        {
            return await _unitOfWork.Notes
                .GetNotesByEntityIdAsync(id);
        }
        public async Task<Note> CreateNote(NotesModel model)
        {
            Note note = new Note();
            var isValid = ValidNote(model);
            if (isValid)
            {
                //Map Model Data
                note.CreatedOn = model.CreatedOn;
                note.CreatedBy = model.CreatedBy;
                note.Notes = model.Notes;
                note.Severity = model.Severity;
                note.Status = model.Status;
                note.EntityId = model.EntityId;
                note.DisplayOnProfile = model.DisplayOnProfile;

                await _unitOfWork.Notes.AddAsync(note);
                await _unitOfWork.CommitAsync();
            }
            return note;
        }

        public async Task<Note> UpdateNote(NotesModel model)
        {
            Note note = await _unitOfWork.Notes.GetNoteByIdAsync(model.NoteId);

            var isValid = ValidNote(model);
            if (isValid)
            {
                //Map Model Data
                note.ModifiedOn = model.ModifiedOn;
                note.ModifiedBy = model.ModifiedBy;
                note.Notes = model.Notes;
                note.Severity = model.Severity;
                note.Status = model.Status;
                note.EntityId = model.EntityId;
                note.DisplayOnProfile = model.DisplayOnProfile;

                _unitOfWork.Notes.Update(note);
                await _unitOfWork.CommitAsync();
            }
            return note;
        }

        public async Task<bool> DeleteNote(int noteId)
        {
            Note note = await _unitOfWork.Notes.GetNoteByIdAsync(noteId);

            if (note != null)
            {
                _unitOfWork.Notes.Remove(note);
                await _unitOfWork.CommitAsync();
                return true;

            }
            throw new InvalidOperationException($"Notes: {noteId} not found.");

        }
        private bool ValidNote(NotesModel model)
        {
            //Validate  Name
            if (model.Notes.IsNullOrEmpty())
            {
                throw new InvalidOperationException($"Note can not be empty.");
            }

            return true;
        }
    }
}
