using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Max.Data.Interfaces;
using Max.Data.Repositories;
using Max.Data.DataModel;
using System;
using Max.Core;
using Max.Core.Helpers;

namespace Max.Data.Repositories
{
    public class QuestionLinkRepository : Repository<Questionlink>, IQuestionLinkRepository
    {
        public QuestionLinkRepository(membermaxContext context)
            : base(context)
        { }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }

        public async Task<Questionlink> GetQuestionLinkByQuestionBankIdAsync(int questionBankId)
        {
            return await membermaxContext.Questionlinks
                .Where(x => x.QuestionBankId == questionBankId)
                .Include(x => x.Event)
                .Include(x => x.Session)
                .SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<Questionlink>> GetQuestionsByEventIdAsync(int eventId)
        {
            return await membermaxContext.Questionlinks
                .Where(x => x.EventId == eventId)
                .Include(x => x.QuestionBank)
                    .ThenInclude(x => x.Answeroptions)
                .Include(x => x.QuestionBank)
                    .ThenInclude(x => x.AnswerTypeLookUp)
                .ToListAsync();
        }

        public async Task<IEnumerable<Questionlink>> GetQuestionsBySessionIdAsync(int sessionId)
        {
            return await membermaxContext.Questionlinks
                 .Where(x => x.SessionId == sessionId)
                .Include(x => x.QuestionBank)
                    .ThenInclude(x => x.Answeroptions)
                .Include(x => x.QuestionBank)
                    .ThenInclude(x => x.AnswerTypeLookUp)
                .ToListAsync();
        }
    }
}
