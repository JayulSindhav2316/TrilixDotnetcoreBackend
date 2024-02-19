﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Max.Data.DataModel;

namespace Max.Data.Interfaces
{
    public interface IItemTypeRepository : IRepository<Itemtype>
    {
        Task<IEnumerable<Itemtype>> GetAllItemTypesAsync();
    }
}
