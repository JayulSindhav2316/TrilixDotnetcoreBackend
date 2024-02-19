using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Max.Core.Models;
using Max.Data.DataModel;
using Max.Data.Interfaces;
using Max.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Max.Services
{
    public class QuestionLinkService : IQuestionLinkService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<QuestionBankService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public QuestionLinkService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<QuestionBankService> logger, IHttpContextAccessor httpContextAccessor)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<Questionlink>> GetQuestionsByEventId(int eventId)
        {
            var questionLinkList = await _unitOfWork.QuestionLinks.GetQuestionsByEventIdAsync(eventId);
            return questionLinkList.ToList();
        }

        public async Task<IEnumerable<QuestionBankModel>> GetQuestionBankByEventId(int eventId)
        {
            var questionLinkList = await _unitOfWork.QuestionLinks.GetQuestionsByEventIdAsync(eventId);
            List<QuestionBankModel> questionBankModelList = new List<QuestionBankModel>();
            foreach (var question in questionLinkList)
            {
                QuestionBankModel questionBankModel = _mapper.Map<QuestionBankModel>(question.QuestionBank);
                questionBankModelList.Add(questionBankModel);
            }
            return questionBankModelList;

        }

        public async Task<bool> LinkQuestionsByEventId(EventModel eventModel)
        {
            var response = false;
            try
            {
                var questionLinkList = await _unitOfWork.QuestionLinks.GetQuestionsByEventIdAsync(eventModel.EventId);
                if(questionLinkList.Count() > 0)
                {
                    _unitOfWork.QuestionLinks.RemoveRange(questionLinkList);
                    await _unitOfWork.CommitAsync();
                }

                var eventQuestionList = eventModel.EventQuestions;
                foreach (var question in eventQuestionList)
                {
                    Questionlink questionlink = new Questionlink();
                    questionlink.EventId = question.EventId;
                    questionlink.QuestionBankId = question.QuestionBankId;
                    await _unitOfWork.QuestionLinks.AddAsync(questionlink);
                }
                await _unitOfWork.CommitAsync();
                response = true;
            }
            
            catch(Exception ex)
            {
                _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                              ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                response = false;
            }

            return response;
        }

        public async Task<IEnumerable<QuestionBankModel>> GetQuestionBankBySessionId(int sessionId)
        {
            var questionLinkList = await _unitOfWork.QuestionLinks.GetQuestionsBySessionIdAsync(sessionId);
            List<QuestionBankModel> questionBankModelList = new List<QuestionBankModel>();
            foreach (var question in questionLinkList)
            {
                QuestionBankModel questionBankModel = _mapper.Map<QuestionBankModel>(question.QuestionBank);
                questionBankModelList.Add(questionBankModel);
            }
            return questionBankModelList;

        }

        public async Task<bool> LinkQuestionsBySessionId(SessionModel model)
        {
            var response = false;
            try
            {
                var questionLinkList = await _unitOfWork.QuestionLinks.GetQuestionsBySessionIdAsync(model.SessionId);
                if (questionLinkList.Count() > 0)
                {
                    _unitOfWork.QuestionLinks.RemoveRange(questionLinkList);
                    await _unitOfWork.CommitAsync();
                }

                var sessionQuestionList = model.SessionQuestions;
                foreach (var question in sessionQuestionList)
                {
                    Questionlink questionlink = new Questionlink();
                    questionlink.SessionId = model.SessionId;
                    questionlink.QuestionBankId = question.QuestionBankId;
                    await _unitOfWork.QuestionLinks.AddAsync(questionlink);
                }
                await _unitOfWork.CommitAsync();
                response = true;
            }

            catch (Exception ex)
            {
                _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                              ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                response = false;
            }

            return response;
        }
    }
}
