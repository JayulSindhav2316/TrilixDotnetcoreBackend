using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Max.Core.Models;
using Max.Data.DataModel;


namespace Max.Services.Interfaces
{
    public interface IAnswerOptionService
    {
        Task<IEnumerable<AnswerOptionModel>> GetAnswerOptionsByQuestionBankId(int questionBankId);
        Task<Answeroption> AddAnswerOption(AnswerOptionModel model);
        Task<bool> UpdateAnswerOption(AnswerOptionModel model);
        Task<bool> DeleteAnswerOption(int answerOptionId);
        Task<AnswerOptionModel> GetAnswerOptionById(int answerOptionId);
    }
}
