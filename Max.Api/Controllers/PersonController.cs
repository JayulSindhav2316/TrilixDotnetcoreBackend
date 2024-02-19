using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Max.Data.DataModel;
using Max.Services.Interfaces;
using Max.Core.Models;
using Max.Core.Helpers;
using Max.Core;
using Max.Services;

namespace Max.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class PersonController : ControllerBase
    {

        private readonly ILogger<PersonController> _logger;
        private readonly IPersonService _personService;
        private readonly IStaffSearchHistoryService _StaffSearchHistoryService;

        public PersonController(IStaffSearchHistoryService staffSearchHistoryService, ILogger<PersonController> logger, IPersonService personService)
        {
            _logger = logger;
            _personService = personService;
            _StaffSearchHistoryService = staffSearchHistoryService;
        }

        [HttpGet("GetPersonById")]
        public async Task<ActionResult<PersonModel>> GetPersonById(int personId)
        {
            if (personId < 0) return BadRequest(new { message = "Invalid Person Id." });
            try
            {
                var person = await _personService.GetPersonById(personId);
                return Ok(person);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("GetAllPersons")]
        public async Task<ActionResult<IEnumerable<Person>>> GetAllPersons()
        {
            var persons = await _personService.GetAllPersons();
            return Ok(persons);
        }

        [HttpGet("GetAllPersonsByQuickSearch")]
        public async Task<ActionResult<SearchModel>> GetAllPersonsByQuickSearch(string quickSearch)
        {
            var user = (Staffuser)HttpContext.Items["StafffUser"];
            await _StaffSearchHistoryService.SaveSearchText(quickSearch, user.UserId);
            var persons = await _personService.GetAllPersonsByQuickSearch(quickSearch);
            return Ok(persons);
        }

        [HttpGet("GetAllPersonsByEmail")]
        public async Task<ActionResult<IEnumerable<PersonModel>>> GetAllPersonsByEmail(string emailAddress,string exceptMemberIds, int exceptedPersonsGroupId = 0)
        {
            var persons = await _personService.GetAllPersonsByEmail(emailAddress, exceptMemberIds, exceptedPersonsGroupId);
            return Ok(persons);
        }

        

        [HttpGet("GetAllPersonsByPhoneNumber")]
        public async Task<ActionResult<IEnumerable<PersonModel>>> GetAllPersonsByPhoneNumber(string phoneNumber,string exceptMemberIds, int exceptedPersonsGroupId = 0)
        {
            var persons = await _personService.GetAllPersonsByPhoneNumber(phoneNumber, exceptMemberIds, exceptedPersonsGroupId);
            return Ok(persons);
        }

        [HttpGet("GetAllPersonsByFirstNameAndLastName")]
        public async Task<ActionResult<IEnumerable<PersonModel>>> GetAllPersonsByFirstNameAndLastName(string firstName, string lastName,string exceptMemberIds, int exceptedPersonsGroupId = 0)
        {
            var persons = await _personService.GetAllPersonsByFirstAndLastName(firstName, lastName, exceptMemberIds, exceptedPersonsGroupId);
            return Ok(persons);
        }

        [HttpGet("GetAllPersonsByNameTitle")]
        public async Task<ActionResult<IEnumerable<PersonModel>>> GetAllPersonsByNameTitle(string text, string exceptMemberIds, int exceptedPersonsGroupId = 0)
        {
            var persons = await _personService.GetAllPersonsByNameTitle(text, exceptMemberIds, exceptedPersonsGroupId);
            return Ok(persons);
        }


        [HttpGet("GetAllPersonsByName")]
        public async Task<ActionResult<IEnumerable<PersonModel>>> GetAllPersonsByName(string name, int exceptedPersonsGroupId = 0)
        {
            var persons = await _personService.GetAllPersonsByName(name, exceptedPersonsGroupId);
            return Ok(persons);
        }

        [HttpGet("GetAllPersonsByIds")]
        public async Task<ActionResult<IEnumerable<PersonModel>>> GetAllPersonsByIds(string personIds)
        {
            if (String.IsNullOrEmpty(personIds)) return BadRequest(new { message = "Invalid Person Id." });

            var persons = await _personService.GetAllPersonsByPersonIds(personIds);
            return Ok(persons);
        }

        
       

        [HttpGet("GetPrimaryAddressByPersonId")]
        public async Task<ActionResult<AddressModel>> GetPrimaryAddressByPersonId(int personId)
        {
            var AddressModel = await _personService.GetPrimaryAddressByPersonId(personId);
            return Ok(AddressModel);
        }

      

        [HttpPost("CreatePerson")]
        public async Task<ActionResult<Person>> CreatePerson(PersonModel personModel)
        {
            Person person = new Person();

            if(ModelState.IsValid ==false)
            {
                return BadRequest(new { message = "Invalid Model Value" });
            }
            try
            {
                person = await _personService.CreatePerson(personModel);
                if (person.PersonId == 0)
                {
                    return BadRequest(new { message = "Failed to create Person" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Create Person: First Name: {personModel.FirstName} Last Name:{personModel.LastName} failed with error {ex.Message} {ex.StackTrace}");
                return BadRequest(new { message = ex.Message });
            }

            return Ok(person);

        }

        [HttpPost("DeletePerson")]
        public async Task<ActionResult<bool>> DeletePerson( PersonModel model)
        {
            bool response = false;
            try
            {
               
                response = await _personService.DeletePerson(model.PersonId);
                if (!response)
                {
                    return BadRequest(new { message = "Sorry, we are not able to delete member profile. Please contact trilix support team." });
                }
               
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(response);
        }

        [HttpPost("UpdatePerson")]
        public async Task<ActionResult<Person>> UpdatePerson(PersonModel personModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Failed to update Person" });
            }
            bool response = false;
            if (personModel.PersonId <= 0) return BadRequest(new { message = "Invalid PersonId supplied." });
            try
            {
                response = await _personService.UpdatePerson(personModel);
                if (!response)
                {
                    return BadRequest(new { message = "Failed to update Person" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                               ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                return BadRequest(new { message = ex.Message });
            }

            return Ok(response);
        }

        

        [HttpGet("GetAllPersonsByMembershipId")]
        public async Task<ActionResult<IEnumerable<PersonModel>>> GetAllPersonsByMembershipId(int membershipId)
        {
            var personModel = await _personService.GetAllPersonsByMembershipId(membershipId);
            return Ok(personModel);
        }


        [HttpGet("GetPeopleByCompanyId")]
        public async Task<ActionResult<IEnumerable<PersonModel>>> GetPeopleByCompanyId(int companyId)
        {
            var personModel = await _personService.GetPeopleByCompanyIdAsync(companyId);
            return Ok(personModel);
        }

        [HttpGet("GetPeopleListByCompanyId")]
        public async Task<ActionResult<IEnumerable<SelectListModel>>> GetPeopleListByCompanyId(int companyId)
        {
            var personModel = await _personService.GetPeopleListByCompanyIdAsync(companyId);
            return Ok(personModel);
        }


        [HttpGet("GetAllPeopleList")]
        public async Task<ActionResult<IEnumerable<SelectListModel>>> GetAllPeopleList()
        {
            var persons = await _personService.GetAllPeopleList();
            return Ok(persons);
        }


        [HttpGet("GetPreferredContact")]
        public ActionResult<IEnumerable<EnumOptionListModel>> GetPreferredContact()
        {
            List<EnumOptionListModel> list = new List<EnumOptionListModel>();
            foreach (int value in Enum.GetValues(typeof(PreferredContact)))
            {
                list.Add(new EnumOptionListModel { Name = EnumUtil.GetDescription(((PreferredContact)value)), Code = value });
            }
            return Ok(list);
        }

        [HttpGet("GetSearchHistory")]
        public ActionResult<IEnumerable<EnumOptionListModel>> GetSearchHistory()
        {
            try
            {
                var user = (Staffuser)HttpContext.Items["StafffUser"];
                var list = _StaffSearchHistoryService.GetSearchHistory(user.UserId);
                return Ok(list);
            }
            catch (Exception ex)
            {

                throw;
            }
           
        }
        [HttpPost("DeleteSearchHistory")]
        public ActionResult<bool> DeleteSearchHistory(Staffusersearchhistory data)
        {
            try
            {
                var user = (Staffuser)HttpContext.Items["StafffUser"];
                var res = _StaffSearchHistoryService.DeleteSearchHistory(data.SearchText,user.UserId);
                return Ok(res);
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        [HttpGet("GetAllPersonsOrCompanyByNameAndEmail")]
        public async Task<ActionResult<IEnumerable<SearchPersonModel>>> GetAllPersonsOrCompanyByNameAndEmail(string value, int entityId, string type)
        {
            try
            {
                var users = await _personService.GetAllPersonsOrCompanyByNameAndEmail(value,entityId,type);
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error. Please try again or contact support team if error persists.");
            }

        }

        [HttpPost("UpdatePersonComapny")]
        public async Task<ActionResult<bool>> UpdatePersonComapny(PersonModel personModel)
        {
            try
            {
                var iscompanyUpdated = await _personService.UpdatePersonComapny(personModel);
                return Ok(iscompanyUpdated);
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        [HttpGet("GetLastAddedPerson")]
        public async Task<ActionResult<PersonModel>> GetLastAddedPerson()
        {
            try
            {
                var person = await _personService.GetLastAddedPerson();
                return Ok(person);
            }
            catch (Exception ex)
            {
                throw;
            }

        }

    }
}
