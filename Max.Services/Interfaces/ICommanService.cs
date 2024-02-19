using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Max.Services.Interfaces
{
    public interface ICommanService
    {
        int? GetUserIdFromContext();
        int? GetOrganizationIdFromContext();
    }
}
