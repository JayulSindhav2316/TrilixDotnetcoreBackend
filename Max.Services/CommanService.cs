using Max.Data.DataModel;
using Max.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Max.Services
{
    public class CommanService : ICommanService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CommanService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int? GetOrganizationIdFromContext()
        {
            var staff = (Staffuser)_httpContextAccessor.HttpContext.Items["StafffUser"];
            var entity = (Entity)_httpContextAccessor.HttpContext.Items["EntityUser"];
            if (staff != null)
            {
                if (staff.OrganizationId > 0)
                {
                    return staff.OrganizationId;
                }
            }
            else if (entity != null)
            {
                if (entity.OrganizationId > 0)
                {
                    return entity.OrganizationId;
                }
            }
            return null;
        }

        public int? GetUserIdFromContext()
        {
            var staff = (Staffuser)_httpContextAccessor.HttpContext.Items["StafffUser"];
            var entity = (Entity)_httpContextAccessor.HttpContext.Items["EntityUser"];
            if (staff != null)
            {
                if (staff.UserId > 0)
                {
                    return staff.UserId;
                }
            }
            else if (entity != null)
            {
                if (entity.EntityId > 0)
                {
                    return entity.EntityId;
                }
            }
            return null;
        }
    }
}
