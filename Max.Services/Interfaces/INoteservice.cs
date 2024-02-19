using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;
using Max.Core.Models;
namespace Max.Services.Interfaces
{
    public interface INoteservice
    {
        Task<IEnumerable<Note>> GetAllNotes();
        Task<Note> GetNoteById(int id);
        Task<Note> CreateNote(NotesModel notesModel);
        Task<IEnumerable<Note>> GetNotesByEntityId(int id);
        Task<Note> UpdateNote(NotesModel notesModel);
        Task<bool> DeleteNote(int noteId);
    }
}
