using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Max.Core.Models;
using Max.Data.DataModel;

namespace Max.Services.Interfaces
{
    public interface IQuestionLinkService
    {
        Task<IEnumerable<Questionlink>> GetQuestionsByEventId(int eventId);
        Task<IEnumerable<QuestionBankModel>> GetQuestionBankByEventId(int eventId);
        Task<bool> LinkQuestionsByEventId(EventModel eventModel);
        Task<IEnumerable<QuestionBankModel>> GetQuestionBankBySessionId(int sessionId);
        Task<bool> LinkQuestionsBySessionId(SessionModel model);
    }
}

