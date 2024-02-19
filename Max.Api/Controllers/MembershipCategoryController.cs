using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Max.Core.Models;
using Max.Data.DataModel;
using Max.Services.Interfaces;

namespace Max.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class MembershipCategoryController : ControllerBase
    {

        private readonly ILogger<MembershipCategoryController> _logger;
        private readonly IMembershipCategoryService _membershipCategoryService;

        public MembershipCategoryController(ILogger<MembershipCategoryController> logger, IMembershipCategoryService MembershipCategoryService)
        {
            _logger = logger;
            _membershipCategoryService = MembershipCategoryService;
        }

        [HttpGet("GetAllMembershipCategorys")]
        public async Task<ActionResult<IEnumerable<Membershipcategory>>> GetAllMembershipCategories()
        {
            var MembershipCategorys = await _membershipCategoryService.GetAllMembershipCategories();
            return Ok(MembershipCategorys);
        }
        [HttpGet("GetSelectList")]
        public async Task<ActionResult<List<SelectListModel>>> GetSelectList()
        {
            var MembershipCategorys = await _membershipCategoryService.GetSelectList();
            return Ok(MembershipCategorys);
        }


    }
}
