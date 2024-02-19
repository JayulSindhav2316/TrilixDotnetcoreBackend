using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Max.Data.DataModel;

namespace Max.Data.Interfaces
{
    public interface IQuestionBankRepository : IRepository<Questionbank>
    {
        Task<IEnumerable<Questionbank>> GetAllQuestionsAsync();
        Task<Questionbank> GetQuestionByIdAsync(int questionBankId);
        Task<Questionbank> GetQuestionByNameAsync(string questionName);
    }
}
