using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Max.Data.DataModel;

namespace Max.Data.Interfaces
{
    public interface IAnswerOptionRepository : IRepository<Answeroption>
    {
        Task<IEnumerable<Answeroption>> GetAnswerOptionsByQuestionBankIdAsync(int questionBankId);
    }
}