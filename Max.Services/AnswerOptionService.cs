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
    public class AnswerOptionService : IAnswerOptionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<QuestionBankService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AnswerOptionService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<QuestionBankService> logger, IHttpContextAccessor httpContextAccessor)
        {
            this._unitOfWork = unitOfWork;
            this._mapper = mapper;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<AnswerOptionModel>> GetAnswerOptionsByQuestionBankId(int questionBankId)
        {
            var answerOptionList = await _unitOfWork.AnswerOptions.GetAnswerOptionsByQuestionBankIdAsync(questionBankId);
            List<AnswerOptionModel> answerOptionModelList = _mapper.Map<List<AnswerOptionModel>>(answerOptionList);
            return answerOptionModelList;

        }

        public async Task<Answeroption> AddAnswerOption(AnswerOptionModel model)
        {
            Answeroption answerOption = new Answeroption();
            answerOption.Option = model.Option;
            answerOption.QuestionBankId = model.QuestionBankId;

            try
            {
                await _unitOfWork.AnswerOptions.AddAsync(answerOption);
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred {Error} {StackTrace} {InnerException} {Source}",
                           ex.Message, ex.StackTrace, ex.InnerException, ex.Source);
            }
            return answerOption;
        }
        public async Task<bool> UpdateAnswerOption(AnswerOptionModel model)
        {
            var answerOption = await _unitOfWork.AnswerOptions.GetByIdAsync(model.AnswerOptionId);

            if (answerOption != null)
            {
                answerOption.Option = model.Option;

                try
                {
                    _unitOfWork.AnswerOptions.Update(answerOption);
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

        public async Task<bool> DeleteAnswerOption(int answerOptionId)
        {
            var answerOption = await _unitOfWork.AnswerOptions.GetByIdAsync(answerOptionId);


            if (answerOption != null)
            {
                _unitOfWork.AnswerOptions.Remove(answerOption);
                await _unitOfWork.CommitAsync();
                return true;
            }
            return false;
        }

        public async Task<AnswerOptionModel> GetAnswerOptionById(int answerOptionId)
        {
            var answerOption = await _unitOfWork.AnswerOptions.GetByIdAsync(answerOptionId);
            AnswerOptionModel answerOptionModel = _mapper.Map<AnswerOptionModel>(answerOption);
            return answerOptionModel;
        }
    }
}
