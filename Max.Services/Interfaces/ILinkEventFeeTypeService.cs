using System;
using System.Collections.Generic;
using System.Text;
using Max.Data.DataModel;
using Max.Core.Models;
using System.Threading.Tasks;

namespace Max.Services.Interfaces
{
    public interface ILinkEventFeeTypeService
    {
        Task<List<LinkEventFeeTypeModel>> GetLinkedFeesByEventId(int eventId);
    }
}
