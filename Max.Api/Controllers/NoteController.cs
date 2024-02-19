using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Max.Data.DataModel;
using Max.Services.Interfaces;
using Max.Core.Models;

namespace Max.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NoteController : ControllerBase
    {

        private readonly ILogger<NoteController> _logger;
        private readonly INoteservice _noteService;

        public NoteController(ILogger<NoteController> logger, INoteservice noteService)
        {
            _logger = logger;
            _noteService = noteService;
        }

        [HttpGet("GetAllNotes")]
        public async Task<ActionResult<IEnumerable<Note>>> GetAllNotes()
        {
            var notes = await _noteService.GetAllNotes();
            return Ok(notes);
        }
        [HttpGet("GetNotesByEntityId")]
        public async Task<ActionResult<IEnumerable<Note>>> GetNotesByEntityId(int entityId)
        {
            var notes = await _noteService.GetNotesByEntityId(entityId);
            return Ok(notes);
        }
        [HttpPost("CreateNotes")]
        public async Task<ActionResult<Note>> CreateNotes(NotesModel model)
        {
            Note note = new Note();

            try
            {
                note = await _noteService.CreateNote(model);
                if (note.NoteId == 0)
                {
                    return BadRequest(new { message = "Failed to create note" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest(new { message = ex.Message });
            }

            return Ok(note);
        }
        [HttpPost("UpdateNotes")]
        public async Task<ActionResult<Note>> UpdateNotes([FromBody] NotesModel model)
        {
            Note note = new Note();

            try
            {
                note = await _noteService.UpdateNote(model);
                if (note.NoteId == 0)
                {
                    _logger.LogError($"Could not find reccord with Noteid ={model.NoteId}");
                    return BadRequest(new { message = "Failed to update note" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                return BadRequest(new { message = ex.Message });
            }

            return Ok(note);
        }
        [HttpPost("DeleteNote")]
        public async Task<ActionResult<Note>> DeleteNote([FromBody] NotesModel model)
        {

            try
            {
                await _noteService.DeleteNote(model.NoteId);

            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                return BadRequest(new { message = ex.Message });
            }

            return Ok(true);
        }


    }
}
