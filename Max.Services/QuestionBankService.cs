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
    public class QuestionBankService : IQuestionBankService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<QuestionBankService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public QuestionBankService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<QuestionBankService> logger, IHttpContextAccessor httpContextAccessor)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<QuestionBankModel>> GetAllQuestions(int status) 
        {
            var questionBankList = await _unitOfWork.QuestionBanks.GetAllQuestionsAsync();
            List<QuestionBankModel> questionBankModelList = _mapper.Map<List<QuestionBankModel>>(questionBankList);
            questionBankModelList = status == 0 ? questionBankModelList : questionBankModelList.Where(x => x.Status == 1).ToList();
            return questionBankModelList;

        }
        public async Task<QuestionBankModel> GetQuestionById(int id)
        {
            var questionBank = await _unitOfWork.QuestionBanks.GetQuestionByIdAsync(id);
            QuestionBankModel questionBankModel = _mapper.Map<QuestionBankModel>(questionBank);
            return questionBankModel;
        }
        public async Task<Questionbank> AddQuestion(QuestionBankModel model)
        {
            Questionbank question = new Questionbank();
            question.Question = model.Question;
            question.AnswerTypeLookUpId = model.AnswerTypeLookUpId;
            question.Status = model.Status;

            try
            { 
                if ((model.AnswerTypeLookUpId == 3 || model.AnswerTypeLookUpId == 4) && model.AnswerOptions.Count == 0)
                {
                    throw new InvalidOperationException($"You need to have atleast one option.");
                }
                else if ((model.AnswerTypeLookUpId == 3 || model.AnswerTypeLookUpId == 4) && model.AnswerOptions.Count > 0)
                {
                    foreach (var option in model.AnswerOptions)
                    {
                        question.Answeroptions.Add(new Answeroption { Option = option.Option });
                    }
                    
                }

                await _unitOfWork.QuestionBanks.AddAsync(question);
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                            ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
            }
            
            return question;
        }
        public async Task<bool> UpdateQuestion(QuestionBankModel model)
        {
            var question = await _unitOfWork.QuestionBanks.GetQuestionByIdAsync(model.QuestionBankId);

            if(question!=null)
            {
                question.Question = model.Question;
                question.AnswerTypeLookUpId = model.AnswerTypeLookUpId;
                question.Status = model.Status;

                if(model.AnswerTypeLookUpId != 3 && model.AnswerTypeLookUpId != 4)
                {
                    if(question.Answeroptions.Count > 0)
                    {
                        _unitOfWork.AnswerOptions.RemoveRange(question.Answeroptions);
                    }

                }
                else if (model.AnswerTypeLookUpId == 3 || model.AnswerTypeLookUpId == 4)
                {
                    if (model.AnswerOptions.Count == 0 && question.Answeroptions.Count == 0)
                    {
                        throw new InvalidOperationException($"You need to have atleast one option.");
                    }

                    foreach (var option in model.AnswerOptions.Where(x => x.AnswerOptionId < 0))
                    {
                        question.Answeroptions.Add(new Answeroption { Option = option.Option });
                    }
                    foreach (var option in model.AnswerOptions.Where(x => x.AnswerOptionId > 0))
                    {
                        Answeroption answeroption = await _unitOfWork.AnswerOptions.GetByIdAsync(option.AnswerOptionId);
                        answeroption.Option = option.Option;
                        _unitOfWork.AnswerOptions.Update(answeroption);
                    }
                }

                try
                {
                    _unitOfWork.QuestionBanks.Update(question);
                    await _unitOfWork.CommitAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                               ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                }
                return true;
            }
            return false;
        }

        public async Task<IEnumerable<QuestionBankModel>> GetQuestionsByStatus(int status)
        {
            var questionBankList = await _unitOfWork.QuestionBanks.GetAllQuestionsAsync();
            var questionsList = questionBankList.Where(x => x.Status == status);
            List<QuestionBankModel> questionBankModelList = _mapper.Map<List<QuestionBankModel>>(questionsList);
            return questionBankModelList;
        }

        public async Task<bool> DeleteQuestion(int questionId)
        {
            var question = await _unitOfWork.QuestionBanks.GetQuestionByIdAsync(questionId);
            
            
            if (question != null)
            {
                if (question.Questionlinks.Count > 0)
                {
                    throw new InvalidOperationException($"Question linked to event / session cannot be deleted.");
                }
                if(question.Answeroptions.Count > 0)
                {
                    _unitOfWork.AnswerOptions.RemoveRange(question.Answeroptions);
                }

                _unitOfWork.QuestionBanks.Remove(question);
                await _unitOfWork.CommitAsync();
                return true;
            }
            return false;
        }

        public async Task<Questionbank> CloneQuestion(int questionBankId)
        {
            var clonedQuestion = new Questionbank();
            var masterQuestion = await _unitOfWork.QuestionBanks.GetQuestionByIdAsync(questionBankId);

            if (masterQuestion != null)
            {
                clonedQuestion.Question = await GenerateCloneQuestionName(masterQuestion.Question);
                clonedQuestion.AnswerTypeLookUpId = masterQuestion.AnswerTypeLookUpId;
                clonedQuestion.Status = masterQuestion.Status;

                foreach (var option in masterQuestion.Answeroptions)
                {
                    clonedQuestion.Answeroptions.Add(new Answeroption { Option = option.Option });
                }

                try
                {
                    await _unitOfWork.QuestionBanks.AddAsync(clonedQuestion);
                    await _unitOfWork.CommitAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}", ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
                    if (ex.InnerException.Message.Contains("Data too long"))
                    {
                        throw new InvalidOperationException($"Question exceeds the max length limit.");
                    }
                    
                }
            }

            return clonedQuestion;
        }

        public async Task<string> GenerateCloneQuestionName(string questionName)
        {
            int i = 1;
            string cloneName = string.Empty;
            Questionbank questionbank = new Questionbank();

            do
            {
                cloneName = questionName + " Copy -" + i.ToString();
                questionbank = await _unitOfWork.QuestionBanks.GetQuestionByNameAsync(cloneName);
                i++;
            }
            while (questionbank != null);
            return cloneName;
        }
    }
}
