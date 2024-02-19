using Max.Data.DataModel;
using Max.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Max.Data.Repositories
{
    public class EventRegisterQuestionRepository : Repository<Eventregisterquestion>, IEventRegisterQuestionRepository
    {
        public EventRegisterQuestionRepository(membermaxContext context)
          : base(context)
        { }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }
    }
}
