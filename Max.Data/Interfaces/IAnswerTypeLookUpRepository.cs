﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Max.Data.DataModel;

namespace Max.Data.Interfaces
{
    public interface IAnswerTypeLookUpRepository : IRepository<Answertypelookup>
    {
        Task<IEnumerable<Answertypelookup>> GetAnswertypelookup();
    }
}
