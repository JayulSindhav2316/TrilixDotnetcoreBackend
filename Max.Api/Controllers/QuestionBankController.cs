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
    public class QuestionBankController : ControllerBase
    {
        private readonly ILogger<QuestionBankController> _logger;
        private readonly IQuestionBankService _questionBankService;
        private readonly IQuestionLinkService _questionLinkService;

        public QuestionBankController(ILogger<QuestionBankController> logger, IQuestionBankService questionBankService, IQuestionLinkService questionLinkService)
        {
            _logger = logger;
            _questionBankService = questionBankService;
            _questionLinkService = questionLinkService;
        }

        [HttpPost("AddQuestion")]
        public async Task<ActionResult<Questionbank>> AddQuestion(QuestionBankModel questionBankModel)
        {
            Questionbank questionbank = new Questionbank();

            if (ModelState.IsValid == false)
            {
                return BadRequest(new { message = "Invalid Model Value" });
            }
            try
            {
                questionbank = await _questionBankService.AddQuestion(questionBankModel);
                if (questionbank.QuestionBankId == 0)
                {
                    return BadRequest(new { message = "Failed to add a question" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(questionbank);

        }

        [HttpDelete("DeleteQuestion/{questionId}")]
        public async Task<ActionResult<bool>> DeleteQuestion(int questionId)
        {
            bool response = false;
            try
            {
                response = await _questionBankService.DeleteQuestion(questionId);
                if (!response)
                {
                    return BadRequest(new { error = "Failed to delete question." });
                }

            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(response);
        }

        [HttpPost("UpdateQuestion")]
        public async Task<ActionResult<bool>> UpdateQuestion(QuestionBankModel questionBankModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Failed to update Question" });
            }
            bool response = false;
            if (questionBankModel.QuestionBankId <= 0) return BadRequest(new { message = "Invalid Question Id supplied." });
            try
            {
                response = await _questionBankService.UpdateQuestion(questionBankModel);
                if (!response)
                {
                    return BadRequest(new { message = "Failed to update Question" });
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

        [HttpGet("CloneQuestion")]
        public async Task<ActionResult<Questionbank>> CloneQuestion(int questionBankId)
        {
            Questionbank questionbank = new Questionbank();

            if (ModelState.IsValid == false)
            {
                return BadRequest(new { message = "Invalid Model Value" });
            }
            try
            {
                questionbank = await _questionBankService.CloneQuestion(questionBankId);
                if (questionbank.QuestionBankId == 0)
                {
                    return BadRequest(new { message = "Failed to clone a question" });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }

            return Ok(questionbank);

        }

        [HttpGet("GetAllQuestions")]
        public async Task<ActionResult<IEnumerable<QuestionBankModel>>> GetAllQuestions(int status)
        {
            var personModel = await _questionBankService.GetAllQuestions(status);
            return Ok(personModel);
        }

        [HttpGet("GetQuestionsByEventId")]
        public async Task<ActionResult<IEnumerable<QuestionBankModel>>> GetQuestionsByEventId(int eventId)
        {
            var questionBankModelList = await _questionLinkService.GetQuestionBankByEventId(eventId);
            return Ok(questionBankModelList);
        }

        [HttpPost("LinkQuestionsByEventId")]
        public async Task<ActionResult<bool>> LinkQuestionsByEventId(EventModel eventModel)
        {
            var response = await _questionLinkService.LinkQuestionsByEventId(eventModel);
            return Ok(response);
        }

        [HttpGet("GetQuestionsBySessionId")]
        public async Task<ActionResult<IEnumerable<QuestionBankModel>>> GetQuestionsBySessionId(int sessionId)
        {
            var questionBankModelList = await _questionLinkService.GetQuestionBankBySessionId(sessionId);
            return Ok(questionBankModelList);
        }

        [HttpPost("LinkQuestionsBySessionId")]
        public async Task<ActionResult<bool>> LinkQuestionsBySessionId(SessionModel sessionModel)
        {
            var response = await _questionLinkService.LinkQuestionsBySessionId(sessionModel);
            return Ok(response);
        }
    }
}
