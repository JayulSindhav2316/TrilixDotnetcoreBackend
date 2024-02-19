using System;
using System.Collections.Generic;
using System.Text;
using Max.Data.DataModel;
using Max.Data.Interfaces;

namespace Max.Data.Repositories
{
    public class RegistrationFeeTypeRepository : Repository<Registrationfeetype>, IRegistrationFeeTypeRepository
    {
        public RegistrationFeeTypeRepository(membermaxContext context)
           : base(context)
        { }

        private membermaxContext membermaxContext
        {
            get { return Context as membermaxContext; }
        }
    }
}
