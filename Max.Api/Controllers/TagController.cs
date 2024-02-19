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

namespace Max.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class TagController : ControllerBase
    {

        private readonly ILogger<TagController> _logger;
        private readonly ITagService _tagService;

        public TagController(ILogger<TagController> logger, ITagService tagService)
        {
            _logger = logger;
            _tagService = tagService;
        }

        [HttpGet("GetAllTags")]
        public async Task<ActionResult<IEnumerable<TagModel>>> GetAllTags()
        {
            var tags = await _tagService.GetAllTags();
            return Ok(tags);
        }

        [HttpGet("GetTagByTagId")]
        public async Task<ActionResult<TagModel>> GetTagByTagId(int tagId)
        {
            var tag = await _tagService.GetTagById(tagId);
            return Ok(tag);
        }


        [HttpPost("CreateTag")]
        public async Task<ActionResult<TagModel>> CreateTag(TagModel model)
        {
            TagModel tag = new TagModel();
            try
            {
                tag = await _tagService.CreateTag(model);
                if (tag.TagId == 0)
                {
                    return BadRequest(new { message = "Failed to create Tag" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            return Ok(tag);
        }

        [HttpPost("UpdateTag")]
        public async Task<ActionResult<TagModel>> UpdateTag(TagModel model)
        {
            bool response = false;
            try
            {
                response = await _tagService.UpdateTag(model);
                if (!response)
                {
                    return BadRequest(new { message = "Failed to update Tag" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            return Ok(response);
        }

        [HttpPost("DeleteTag")]
        public async Task<ActionResult<bool>> DeleteTag(TagModel model)
        {
            bool response = false;
            try
            {
                response = await _tagService.DeleteTag(model.TagId);
                if (!response)
                {
                    return BadRequest(new { message = "Failed to delete Tag" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(response);
        }

        [HttpGet("GetTagSelectList")]
        public async Task<ActionResult<IEnumerable<SelectListModel>>> GetTagSelectList()
        {
            var selectList = await _tagService.GetTagList();
            return Ok(selectList);
        }
    }
}
