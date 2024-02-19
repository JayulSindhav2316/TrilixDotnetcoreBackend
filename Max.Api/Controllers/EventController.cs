using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Max.Core.Models;
using Max.Data.Interfaces;
using Max.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Max.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EventController : ControllerBase
    {
        string rootFolder = string.Empty;
        private readonly ILogger<EventController> _logger;
        private readonly IHostEnvironment _appEnvironment;
        private readonly IEventService _eventService;
        private readonly IGroupRegistrationService _groupRegistrationService;
        private readonly ILinkEventFeeTypeService _linkEventFeeTypeService;
        public EventController(ILogger<EventController> logger, IHostEnvironment appEnvironment, IEventService eventService, IGroupRegistrationService groupRegistrationService, ILinkEventFeeTypeService linkEventFeeTypeService)
        {
            _logger = logger;
            _appEnvironment = appEnvironment;
            rootFolder = _appEnvironment.ContentRootPath;
            _eventService = eventService;
            _groupRegistrationService = groupRegistrationService;
            _linkEventFeeTypeService = linkEventFeeTypeService;
        }

        [HttpGet("GetEventModel")]
        public EventModel GetEventModel()
        {
            EventModel eventModel = new EventModel();
            return eventModel;
        }

        [HttpPost("CreateEvent")]
        public async Task<ActionResult<EventModel>> CreateEvent(EventModel eventModel)
        {
            EventModel response = new EventModel();
            try
            {
                response = await _eventService.CreateEvent(eventModel);
                if (response.EventId == 0)
                {
                    return BadRequest(new { message = "Failed to create event" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                return BadRequest(new { message = ex.Message });
            }

            return Ok(response);
        }

        [HttpPost("UpdateEvent")]
        public async Task<ActionResult<EventModel>> UpdateEvent(EventModel eventModel)
        {
            var response = false;
            try
            {
                response = await _eventService.UpdateEvent(eventModel);
                if (!response)
                {
                    return BadRequest(new { message = "Failed to update event" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                return BadRequest(new { message = ex.Message });
            }

            return Ok(response);
        }

        [HttpDelete("DeleteEvent/{id}")]
        public async Task<ActionResult<bool>> DeleteEvent(int id)
        {
            bool response = false;
            try
            {
                response = await _eventService.DeleteEvent(id);
                if (!response)
                {
                    return BadRequest(new { message = "Failed to delete event." });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(response);
        }

        [HttpGet("CloneEvent")]
        public async Task<ActionResult<bool>> CloneEvent(int eventId)
        {
            bool response = false;
            try
            {
                response = await _eventService.CloneEvent(eventId);
                if (!response)
                {
                    return BadRequest(new { message = "Failed to clone event." });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(response);
        }

        [HttpPost("UpdateEventSettings")]
        public async Task<ActionResult<EventModel>> UpdateEventSettings(EventModel eventModel)
        {
            var response = false;
            try
            {
                response = await _eventService.UpdateEventSettings(eventModel);
                if (!response)
                {
                    return BadRequest(new { message = "Failed to update event settings" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                return BadRequest(new { message = ex.Message });
            }

            return Ok(response);
        }


        [HttpGet("GetAllEvents")]
        public async Task<ActionResult<IEnumerable<EventModel>>> GetAllEvents()
        {
            var events = await _eventService.GetAllEvents();
            return Ok(events);
        }

        [HttpGet("GetEventsByFilter")]
        public async Task<ActionResult<IEnumerable<EventListModel>>> GetEventsByFilter(DateTime filterDate, int filter = 1)
        {
            var events = await _eventService.GetEventsByFilter(filterDate, filter);
            return Ok(events);
        }

        [HttpGet("GetEventDetailsById")]
        public async Task<ActionResult<EventModel>> GetEventDetailsById(int eventId)
        {
            var events = await _eventService.GetEventDetailsById(eventId);
            var linkfeetypes = await _linkEventFeeTypeService.GetLinkedFeesByEventId(eventId);
            var linkEventGroupModelList = await _groupRegistrationService.GetLinkEventModelForAllRegisterGroups(eventId);
            events.LinkedGroups = linkEventGroupModelList.ToList();
            events.LinkedFeeTypes = linkfeetypes;
            return Ok(events);
        }

        [HttpGet("GetEventBasicDetailsById")]
        public async Task<ActionResult<EventModel>> GetEventBasicDetailsById(int eventId)
        {
            var events = await _eventService.GetEventBasicDetailsById(eventId);
            return Ok(events);
        }

        [HttpGet("GetEventSettingsById")]
        public async Task<ActionResult<EventModel>> GetEventSettingsById(int eventId)
        {
            var events = await _eventService.GetEventBasicDetailsById(eventId);
            var linkfeetypes = await _linkEventFeeTypeService.GetLinkedFeesByEventId(eventId);
            var linkEventGroupModelList = await _groupRegistrationService.GetLinkEventModelForAllRegisterGroups(eventId);
            events.LinkedGroups = linkEventGroupModelList.ToList();
            events.LinkedFeeTypes = linkfeetypes;
            return Ok(events);
        }

        [HttpGet("GetAllActiveEvents/{includePastEvents}")]
        public async Task<ActionResult<IEnumerable<EventModel>>> GetAllActiveEvents(bool includePastEvents)
        {
            var events = await _eventService.GetAllActiveEvents(includePastEvents);
            return Ok(events);
        }

        [HttpPost("CreateEventRegister")]
        public async Task<ActionResult<int>> CreateEventRegister(EventRegisterModel eventRegisterModel)
        {
            var created = await _eventService.CreateEventRegister(eventRegisterModel);
            return Ok(created);
        }

        [HttpGet("CheckEventRegistrationByEventId")]
        public async Task<ActionResult<bool>> CheckEventRegistrationByEventId(int eventId)
        {
            var events = await _eventService.CheckEventRegistrationByEventId(eventId);
            return Ok(events);
        }
    }
}
