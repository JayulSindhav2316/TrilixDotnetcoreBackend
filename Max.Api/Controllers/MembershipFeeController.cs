using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Max.Data.DataModel;
using Max.Services;
using Max.Core.Models;
using Max.Services.Interfaces;
using Max.Core;
using Max.Core.Helpers;

namespace Max.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class MembershipFeeController : ControllerBase
    {

        private readonly ILogger<MembershipFeeController> _logger;
        private readonly IMembershipFeeService _membershiFeeService;
        private readonly IBillingFeeService _billingFeeService;

        public MembershipFeeController(ILogger<MembershipFeeController> logger, IMembershipFeeService membershipFeeService, IBillingFeeService billingFeeService)
        {
            _logger = logger;
            _membershiFeeService = membershipFeeService;
            _billingFeeService = billingFeeService;
        }

        [HttpGet("GetAllMembershipFees")]
        public async Task<ActionResult<IEnumerable<Membershipfee>>> GetAllMembershipFees()
        {
            var membershipFees = await _membershiFeeService.GetAllMembershipFees();
            return Ok(membershipFees);
        }

        [HttpGet("GetBillingFrequency")]
        public  ActionResult<IEnumerable<EnumOptionListModel>> GetBillingFrequency()
        {
            List<EnumOptionListModel> list = new List<EnumOptionListModel>();
            foreach (int value in Enum.GetValues(typeof(FeeBillingFrequency)))
            {
                list.Add( new EnumOptionListModel { Name = EnumUtil.GetDescription(((FeeBillingFrequency)value)), Code = value });
            }
            return Ok(list);
        }

        [HttpGet("GetMembershipFeesByMembershipType")]
        public async Task<ActionResult<IEnumerable<MembershipFeeModel>>> GetMembershipFeesByMembershipType(int membershipTypeId)
        {
            if( ModelState.IsValid)
            {
                var membershipFees = await _membershiFeeService.GetMembershipFeesByMembershipTypeId(membershipTypeId);
                return Ok(membershipFees);
            }
            return BadRequest(new { message = "Invalid Membership Type." });
        }
        [HttpGet("GetBillingFeeByMembershipId")]
        public async Task<ActionResult<IEnumerable<BillingFeeModel>>> GetBillingFeeByMembershipId(int membershipId)
        {
            if (ModelState.IsValid)
            {
                var billingFees = await _billingFeeService.GetBillingFeeByMembershipId(membershipId);
                return Ok(billingFees);
            }
            return BadRequest(new { message = "Invalid Membership Type." });
        }

        [HttpGet("GetMembershipFeesByIds")]
        public async Task<ActionResult<IEnumerable<MembershipFeeModel>>> GetMembershipFeesByIds(string feeIds)
        {
            if(String.IsNullOrEmpty(feeIds)) return BadRequest(new { message = "Invalid Membership Fee Id." });

            if (ModelState.IsValid)
            {
                var membershipFees = await _membershiFeeService.GetMembershipFeesByFeeIds(feeIds);
                return Ok(membershipFees);
            }
            return BadRequest(new { message = "Invalid Membership Fee Id." });
        }

        [HttpPost("DeleteMembershipFee")]
        public async Task<ActionResult<bool>> DeleteMembershipFee(MembershipFeeModel model)
        {
            bool response = false;
            try
            {
                response = await _membershiFeeService.DeleteMembershipFee(model.FeeId);
                if (!response)
                {
                    return BadRequest(new { message = "Failed to delete Membership Fee" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(response);
        }

    }
}
