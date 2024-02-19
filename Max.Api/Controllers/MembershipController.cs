using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Max.Data.DataModel;
using Max.Services.Interfaces;
using Max.Core.Models;
using Max.Services;
using System.Linq;
using Max.Core;
using Max.Core.Helpers;

namespace Max.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class MembershipController : ControllerBase
    {

        private readonly ILogger<MembershipController> _logger;
        private readonly IMembershipService _membershipService;
        private readonly IInvoiceService _invoiceService;

        public MembershipController(ILogger<MembershipController> logger, IMembershipService membershipService, IInvoiceService invoiceService)
        {
            _logger = logger;
            _membershipService = membershipService;
            _invoiceService = invoiceService;
        }

        [HttpGet("GetAllMemberships")]
        public async Task<ActionResult<IEnumerable<Membership>>> GetAllMemberships()
        {
            var memberships = await _membershipService.GetAllMemberships();
            return Ok(memberships);
        }

        [HttpGet("GetMembershipEndDate")]
        public async Task<ActionResult<IEnumerable<Membership>>> GetMembershipEndDate(int periodId, DateTime startDate)
        {
            var memberships = await _membershipService.GetMembershipEndDate(periodId, startDate);
            return Ok(memberships);
        }
        [HttpPost("CreateMembership")]
        public async Task<ActionResult<Membership>> CreateMembership([FromBody] MembershipModel model)
        {
            Membership response = new Membership();

            try
            {
                response = await _membershipService.CreateMembership(model);
                if (response.MembershipId == 0)
                {
                    return BadRequest(new { message = "Failed to create Membership" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                return BadRequest(new { message = ex.Message });
            }

            return Ok(response);
        }
        [HttpPost("CreateNewMembership")]
        public async Task<ActionResult<InvoiceModel>> CreateNewMembership([FromBody] MembershipSessionModel model)
        {
            if(!ModelState.IsValid)
            {
                string messages = string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));
                _logger.LogError($"Invalid Model state. {messages}");
            }

            try
            {
                var membershipModel = await _membershipService.CreateNewMembership(model);
                if (membershipModel.MembershipId == 0)
                {
                    return BadRequest(new { message = "Failed to create Membership" });
                }
                model.MembershipId = membershipModel.MembershipId;
                var invoiceModel = await _invoiceService.CreateNewMembershipInvoice(model);
                return Ok(invoiceModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                return BadRequest(new { message = ex.Message });
            }

            
        }
        [HttpPost("UpdateMembership")]
        public async Task<ActionResult<Membership>> UpdateMembership([FromBody] MembershipModel model)
        {
            Membership response = new Membership();

            try
            {
                response = await _membershipService.UpdateMembership(model);
                if (response.MembershipId == 0)
                {
                    _logger.LogError("$Could not find reccord with MembershipId={model.MembershipId}");
                    return BadRequest(new { message = "Failed to update Membership" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                return BadRequest(new { message = ex.Message });
            }

            return Ok(response);
        }
        [HttpPost("UpdateMembershipDetails")]
        public async Task<ActionResult<Membership>> UpdateMembershipDetails([FromBody] MembershipEditModel model)
        {
            Membership response = new Membership();

            try
            {
                response = await _membershipService.UpdateMembershipDetails(model);
                if (response.MembershipId == 0)
                {
                    _logger.LogError("$Could not find reccord with MembershipId={model.MembershipId}");
                    return BadRequest(new { message = "Failed to update Membership" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                return BadRequest(new { message = ex.Message });
            }

            return Ok(response);
        }
        [HttpPost("DeleteMembership")]
        public async Task<ActionResult<bool>> DeleteMembership([FromBody] MembershipModel model)
        {

            try
            {
                await _membershipService.DeleteMembership(model.MembershipId);

            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                return BadRequest(new { message = ex.Message });
            }

            return Ok(true);
        }

        [HttpPost("TerminateMembership")]
        public async Task<ActionResult<bool>> TerminateMembership([FromBody] MembershipChangeModel model)
        {

            try
            {
                await _membershipService.TerminateMembership(model);

            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                return BadRequest(new { message = ex.Message });
            }

            return Ok(true);
        }
        [HttpPost("CancelNewMembership")]
        public async Task<ActionResult<bool>> CancelNewMembership([FromBody] MembershipCancelModel model)
        {

            try
            {
                await _membershipService.CancelNewMembership(model);

            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                               ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                return BadRequest(new { message = ex.Message });
            }

            return Ok(true);
        }

        [HttpGet("GetMembershipStatus")]
        public ActionResult<IEnumerable<EnumOptionListModel>> GetMembershipStatus()
        {
            List<EnumOptionListModel> list = new List<EnumOptionListModel>();
            foreach (int value in Enum.GetValues(typeof(MembershipStatus)))
            {
                list.Add(new EnumOptionListModel { Name = EnumUtil.GetDescription(((MembershipStatus)value)), Code = value });
            }
            return Ok(list);
        }

        [HttpGet("GetMembershipTypeStatus")]
        public ActionResult<IEnumerable<EnumOptionListModel>> GetMembershipTypeStatus()
        {
            List<EnumOptionListModel> list = new List<EnumOptionListModel>();
            foreach (int value in Enum.GetValues(typeof(Status)))
            {
                list.Add(new EnumOptionListModel { Name = EnumUtil.GetDescription(((Status)value)), Code = value });
            }
            return Ok(list);
        }

    }
}
