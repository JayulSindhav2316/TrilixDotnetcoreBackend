using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Max.Data.DataModel;
using Max.Services.Interfaces;
using Max.Core.Models;
using System.Linq;

namespace Max.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class ItemController : ControllerBase
    {

        private readonly ILogger<ItemController> _logger;
        private readonly IItemService _itemService;

        public ItemController(ILogger<ItemController> logger, IItemService itemService)
        {
            _logger = logger;
            _itemService = itemService;
        }

        [HttpGet("GetAllItems")]
        public async Task<ActionResult<IEnumerable<ItemModel>>> GetAllItems(int status)
        {
            var items = await _itemService.GetAllItems(status);
            return Ok(items);
        }

        [HttpGet("GetItemsByCode")]
        public async Task<ActionResult<IEnumerable<ItemModel>>> GetItemsByCode(string itemCode, int status)
        {
            var items = await _itemService.GetItemsByCode(itemCode, status);
            return Ok(items);
        }

        [HttpGet("GetItemsByName")]
        public async Task<ActionResult<IEnumerable<ItemModel>>> GetItemsByName(string name, int status)
        {
            var items = await _itemService.GetItemsByName(name, status);
            return Ok(items);
        }

        [HttpGet("GetItemById")]
        public async Task<ActionResult<ItemModel>> GetItemById(int itemId)
        {

            var item = await _itemService.GetItemById(itemId);
            return Ok(item);
        }

        [HttpPost("CreateItem")]
        public async Task<ActionResult<Item>> CreateItem(ItemModel model)
        {
            Item response = new Item();
            if (!ModelState.IsValid)
            {
                string messages = string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));
                _logger.LogError($"Invalid Model state. {messages}");
            }
            try
            {
                response = await _itemService.CreateItem(model);
                if (response.ItemId == 0)
                {
                    return BadRequest(new { message = "Failed to create Item" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                return BadRequest(new { message = ex.Message });
            }

            return Ok(response);
        }
        [HttpPost("UpdateItem")]
        public async Task<ActionResult<Item>> UpdateItem( ItemModel model)
        {
            Item response = new Item();

            try
            {
                response = await _itemService.UpdateItem(model);
                if (response.ItemId == 0)
                {
                    _logger.LogError("$Could not find reccord with ItemId={model.ItemId}");
                    return BadRequest(new { message = "Failed to update Item" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                return BadRequest(new { message = ex.Message });
            }

            return Ok(response);
        }
        [HttpPost("DeleteItem")]
        public async Task<ActionResult<Item>> DeleteItem([FromBody] ItemModel model)
        {

            try
            {
                await _itemService.DeleteItem(model.ItemId);

            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex.Message}");
                return BadRequest(new { message = ex.Message });
            }

            return Ok(true);
        }
        [HttpGet("GetSelectList")]
        public async Task<ActionResult<List<SelectListModel>>> GetSelectList()
        {
            var itemTypes = await _itemService.GetSelectList();
            return Ok(itemTypes);
        }
    }
}
