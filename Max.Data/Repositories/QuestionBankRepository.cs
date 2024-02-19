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
    public class QuestionBankRepository : Repository<Questionbank>, IQuestionBankRepository
    {
        public QuestionBankRepository(membermaxContext context)
            : base(context)
        { }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }

        public async Task<IEnumerable<Questionbank>> GetAllQuestionsAsync()
        {
            return await  membermaxContext.Questionbanks
                .Include(x => x.Answeroptions)
                .Include(X => X.AnswerTypeLookUp)
                .OrderByDescending(x => x.QuestionBankId)
                .ToListAsync();
        }

        public async Task<Questionbank> GetQuestionByIdAsync(int questionBankId)
        {
            return await membermaxContext.Questionbanks
                .Include(x => x.Answeroptions)
                .Include(x => x.Questionlinks)
                .Where(x => x.QuestionBankId == questionBankId)
                .SingleOrDefaultAsync();
        }

        public async Task<Questionbank> GetQuestionByNameAsync(string questionName)
        {
            return await membermaxContext.Questionbanks
                .Include(x => x.Answeroptions)
                .Include(x => x.Questionlinks)
                .Where(x => x.Question == questionName)
                .SingleOrDefaultAsync();
        }
    }
}
