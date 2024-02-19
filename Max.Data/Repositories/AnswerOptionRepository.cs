using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Max.Data.DataModel;
using Max.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Max.Data.Repositories
{
    public class AnswerOptionRepository : Repository<Answeroption>, IAnswerOptionRepository
    {
        public AnswerOptionRepository(membermaxContext context)
           : base(context)
        { }

        public async Task<IEnumerable<Answeroption>> GetAnswerOptionsByQuestionBankIdAsync(int questionBankId)
        {
            return await membermaxContext.Answeroptions
                .Where(x => x.QuestionBankId == questionBankId)
                .Include(x => x.QuestionBank)
                .ToListAsync();
        }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }
    }
}
