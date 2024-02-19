using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Max.Data.DataModel;

namespace Max.Data.Interfaces
{
    public interface IQuestionLinkRepository : IRepository<Questionlink>
    {
        Task<Questionlink> GetQuestionLinkByQuestionBankIdAsync(int questionBankId);
        Task<IEnumerable<Questionlink>> GetQuestionsByEventIdAsync(int eventId);
        Task<IEnumerable<Questionlink>> GetQuestionsBySessionIdAsync(int sessionId);
    }
}
