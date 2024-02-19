using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Max.Data.DataModel;
using Max.Services.Interfaces;
using Max.Core.Models;
using Newtonsoft.Json;
using System.Linq;

namespace Max.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class LookupController : ControllerBase
    {

        private readonly ILogger<LookupController> _logger;
        private readonly ILookupService _LookupService;

        public LookupController(ILogger<LookupController> logger, ILookupService LookupService)
        {
            _logger = logger;
            _LookupService = LookupService;
        }

        [HttpGet("GetAllLookups")]
        public async Task<ActionResult<IEnumerable<Lookup>>> GetAllLookups()
        {
            var Lookups = await _LookupService.GetAllLookups();
            return Ok(Lookups);
        }

        [HttpGet("GetLookupValues")]
        public async Task<ActionResult<IEnumerable<Lookup>>> GetLookupValues(string name)
        {
            List<SelectListModel> selectList = new List<SelectListModel>();

            var LookUpValues = await _LookupService.GetLookupValueByGroupName(name);

            string[] optionList = JsonConvert.DeserializeObject<string[]>(LookUpValues);

            foreach (string item in optionList)
            {
                var selectItem = new SelectListModel();
                selectItem.code = item;
                selectItem.name = item;
                selectList.Add(selectItem);
            }
            return Ok(selectList);
        }

        [HttpPost("CreateLookup")]
        public async Task<ActionResult<Lookup>> CreateLookup(LookupModel LookupModel)
        {
            Lookup Lookup = new Lookup();

            try
            {
                Lookup = await _LookupService.CreateLookup(LookupModel);
                if (Lookup.LookupId == 0)
                {
                    return BadRequest(new { message = "Failed to create Lookup" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(Lookup);

        }

        [HttpPost("DeleteLookup")]
        public async Task<ActionResult<bool>> DeleteLookup(LookupModel model)
        {
            bool response = false;
            try
            {
                response = await _LookupService.DeleteLookup(model.LookupId);
                if (!response)
                {
                    return BadRequest(new { message = "Failed to delete Staff User" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(response);
        }

        [HttpPost("UpdateLookup")]
        public async Task<ActionResult<Lookup>> UpdateLookup(LookupModel LookupModel)
        {
            bool response = false;

            try
            {
                response = await _LookupService.UpdateLookup(LookupModel);
                if (!response)
                {
                    return BadRequest(new { message = "Failed to update Lookup" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(response);
        }

        [HttpGet("GetAnswerTypeLookup")]
        public async Task<ActionResult<IEnumerable<DropdownListModel>>> GetAnswerTypeLookup()
        {
            List<DropdownListModel> dropdownList = new List<DropdownListModel>();

            var lookUpValues = await _LookupService.GetAnswerTypeLookup();

            foreach (var item in lookUpValues)
            {
                DropdownListModel dropdownListModel = new DropdownListModel();
                dropdownListModel.value = item.AnswerTypeLookUpId;
                dropdownListModel.name = item.AnswerType;
                dropdownList.Add(dropdownListModel);
            }

            return Ok(dropdownList.OrderBy(x => x.value));
        }

        [HttpGet("GetEventTypeLookup")]
        public async Task<ActionResult<IEnumerable<DropdownListModel>>> GetEventTypeLookup()
        {
            List<DropdownListModel> dropdownList = new List<DropdownListModel>();

            var lookUpValues = await _LookupService.GetEventTypeLookup();

            foreach (var item in lookUpValues)
            {
                DropdownListModel dropdownListModel = new DropdownListModel();
                dropdownListModel.value = item.EventTypeId;
                dropdownListModel.name = item.EventType1;
                dropdownList.Add(dropdownListModel);
            }

            return Ok(dropdownList.OrderBy(x => x.value));
        }

        [HttpGet("GetTimeZonesLookup")]
        public async Task<ActionResult<IEnumerable<DropdownListModel>>> GetTimeZonesLookup()
        {
            List<DropdownListModel> dropdownList = new List<DropdownListModel>();

            var lookUpValues = await _LookupService.GetTimezoneLookup();

            foreach (var item in lookUpValues)
            {
                DropdownListModel dropdownListModel = new DropdownListModel();
                dropdownListModel.value = item.TimeZoneId;
                var offSet = item.TimeZoneOffset > 0 ? "+" + item.TimeZoneOffset.ToString() : item.TimeZoneOffset.ToString();
                dropdownListModel.name = item.TimeZoneAbbreviation + " (" + offSet + ")";
                dropdownList.Add(dropdownListModel);
            }

            return Ok(dropdownList.OrderBy(x => x.value));
        }

        [HttpGet("GetRegistrationFeeTypes")]
        public async Task<ActionResult<IEnumerable<LinkEventFeeTypeModel>>> GetRegistrationFeeTypes()
        {
            List<LinkEventFeeTypeModel> dropdownList = new List<LinkEventFeeTypeModel>();

            var lookUpValues = await _LookupService.GetRegistrationFeeTypes();

            foreach (var item in lookUpValues)
            {
                LinkEventFeeTypeModel dropdownListModel = new LinkEventFeeTypeModel();
                dropdownListModel.RegistrationFeeTypeId = item.RegistrationFeeTypeId;
                dropdownListModel.RegistrationFeeTypeName = item.RegistrationFeeTypeName;
                dropdownList.Add(dropdownListModel);
            }

            return Ok(dropdownList.OrderBy(x => x.RegistrationFeeTypeId));
        }
    }
}
