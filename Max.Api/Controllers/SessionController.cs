using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Max.Core.Models;
using Max.Data.DataModel;
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
    public class SessionController : ControllerBase
    {
        string rootFolder = string.Empty;
        private readonly ILogger<SessionController> _logger;
        private readonly IHostEnvironment _appEnvironment;
        private readonly IEventService _eventService;
        private readonly ISessionService _sessionService;

        public SessionController(ILogger<SessionController> logger, IHostEnvironment appEnvironment, IEventService eventService, ISessionService sessionService)
        {
            _logger = logger;
            _appEnvironment = appEnvironment;
            rootFolder = _appEnvironment.ContentRootPath;
            _eventService = eventService;
            _sessionService = sessionService;
        }

        [HttpGet("GetAllSessionsByEventId")]
        public async Task<ActionResult<IEnumerable<SessionModel>>> GetAllSessionsByEventId(int eventId)
        {
            var sessions = await _sessionService.GetAllSessionsByEventId(eventId);
            return Ok(sessions);
        }

        [HttpPost("CreateSession")]
        public async Task<ActionResult<EventModel>> CreateSession(SessionModel sessionModel)
        {
            SessionModel response = new SessionModel();
            try
            {
                response = await _sessionService.CreateSession(sessionModel);
                if (response.SessionId == 0)
                {
                    return BadRequest(new { message = "Failed to create session" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                return BadRequest(new { message = ex.Message });
            }

            return Ok(response);
        }

        [HttpPost("UpdateSession")]
        public async Task<ActionResult<EventModel>> UpdateSession(SessionModel sessionModel)
        {
            bool response = false;
            try
            {
                response = await _sessionService.UpdateSession(sessionModel);
                if (!response)
                {
                    return BadRequest(new { message = "Failed to update session" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                return BadRequest(new { message = ex.Message });
            }

            return Ok(response);
        }

        [HttpGet("GetSessionById")]
        public async Task<ActionResult<SessionModel>> GetSessionById(int sessionId)
        {
            var session = await _sessionService.GetSessionById(sessionId);
            return Ok(session);
        }

        [HttpGet("GetNewSessionModel")]
        public async Task<ActionResult<SessionModel>> GetNewSessionModel(int eventId)
        {
            var session = await _sessionService.GetNewSessionModel(eventId);
            return Ok(session);
        }

        [HttpDelete("DeleteSession/{sessionId}")]
        public async Task<ActionResult<bool>> DeleteSession(int sessionId)
        {
            bool response = false;
            try
            {
                var regsitration=await _sessionService.GetRegisteredSessions(sessionId);
                if (regsitration.Count()>0)
                {
                    return BadRequest(new { message = "You cannot delete session(s) which has registrations." });
                }
                response = await _sessionService.DeleteSession(sessionId);
                if (!response)
                {
                    return BadRequest(new { message = "Failed to delete session." });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(response);
        }

        [HttpGet("CloneSession")]
        public async Task<ActionResult<bool>> CloneSession(int sessionId)
        {
            bool response = false;
            try
            {
                response = await _sessionService.CloneSession(sessionId);
                if (!response)
                {
                    return BadRequest(new { message = "Failed to clone session." });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(response);
        }

        [HttpGet("GetSessionLeadersBySessionId")]
        public async Task<ActionResult<SessionLeaderLinkModel>> GetSessionLeadersBySessionId(int eventId)
        {
            var tenantId = HttpContext.Request.Headers["X-Tenant-Id"].FirstOrDefault();
            string rootFolder = _appEnvironment.ContentRootPath;
            var sessionLeaders = await _sessionService.GetSessionLeadersBySessionId(rootFolder, tenantId, eventId);
            return Ok(sessionLeaders);
        }

        [HttpGet("GetEventRegistrationSessionGroupAndPricing")]
        public async Task<ActionResult<IEnumerable<EventRegistrationSessionGroupAndPricingModel>>> GetEventRegistrationSessionGroupAndPricing(int eventId, int entityId)
        {
            var groupAndPricings = await _sessionService.GetEventRegistrationSessionGroupAndPricing(eventId, entityId);
            return Ok(groupAndPricings);
        }

        [HttpGet("GetRegisteredSessionsByEntity")]
        public async Task<ActionResult<IEnumerable<string>>> GetRegisteredSessionsByEntity(int eventId, int entityId,string sessionIds)
        {
            var registeredSessions = await _sessionService.GetRegisteredSessionsByEntity(eventId,entityId, sessionIds);
            return Ok(registeredSessions);
        }
    }
}
