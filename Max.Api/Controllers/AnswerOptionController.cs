using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Max.Core.Models;
using Max.Data.DataModel;
using Max.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Max.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AnswerOptionController : ControllerBase
    {
        private readonly ILogger<AnswerOptionController> _logger;
        private readonly IAnswerOptionService _answerOptionService;

        public AnswerOptionController(ILogger<AnswerOptionController> logger, IAnswerOptionService answerOptionService)
        {
            _logger = logger;
            _answerOptionService = answerOptionService;
        }

        [HttpPost("AddAnswerOption")]
        public async Task<ActionResult<Answeroption>> AddAnswerOption(AnswerOptionModel answerOptionModel)
        {
            Answeroption answeroption = new Answeroption();

            if (ModelState.IsValid == false)
            {
                return BadRequest(new { message = "Invalid Model Value" });
            }
            try
            {
                answeroption = await _answerOptionService.AddAnswerOption(answerOptionModel);
                if (answeroption.AnswerOptionId == 0)
                {
                    return BadRequest(new { message = "Failed to add a option" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(answeroption);

        }

        [HttpDelete("DeleteAnswerOption/{answerOptionId}")]
        public async Task<ActionResult<bool>> DeleteAnswerOption(int answerOptionId)
        {
            bool response = false;
            try
            {
                response = await _answerOptionService.DeleteAnswerOption(answerOptionId);
                if (!response)
                {
                    return BadRequest(new { error = "Failed to delete option." });
                }

            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(response);
        }

        [HttpPost("UpdateAnswerOption")]
        public async Task<ActionResult<bool>> UpdateAnswerOption(AnswerOptionModel answerOptionModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Failed to update option" });
            }
            bool response = false;
            if (answerOptionModel.AnswerOptionId <= 0) return BadRequest(new { message = "Invalid option Id supplied." });
            try
            {
                response = await _answerOptionService.UpdateAnswerOption(answerOptionModel);
                if (!response)
                {
                    return BadRequest(new { message = "Failed to update option" });
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

        [HttpGet("GetAnswerOptionsByQuestionBankId")]
        public async Task<ActionResult<IEnumerable<AnswerOptionModel>>> GetAnswerOptionsByQuestionBankId(int questionBankId)
        {
            var answerOptions = await _answerOptionService.GetAnswerOptionsByQuestionBankId(questionBankId);
            return Ok(answerOptions);
        }

        [HttpGet("GetAnswerOptionById")]
        public async Task<ActionResult<AnswerOptionModel>> GetAnswerOptionById(int answerOptionId)
        {
            var answerOption = await _answerOptionService.GetAnswerOptionById(answerOptionId);
            return Ok(answerOption);
        }
    }
}
