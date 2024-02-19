using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Max.Core.Models;
using Max.Data.DataModel;

namespace Max.Services.Interfaces
{
    public interface IQuestionBankService
    {
        Task<IEnumerable<QuestionBankModel>> GetAllQuestions(int status);
        Task<QuestionBankModel> GetQuestionById(int id);
        Task<Questionbank> AddQuestion(QuestionBankModel model);
        Task<bool> UpdateQuestion(QuestionBankModel model);
        Task<IEnumerable<QuestionBankModel>> GetQuestionsByStatus(int status);
        Task<bool> DeleteQuestion(int questionId);
        Task<Questionbank> CloneQuestion(int questionBankId);
    }
}
