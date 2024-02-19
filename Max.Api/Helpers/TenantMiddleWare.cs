using Max.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Threading.Tasks;

namespace Max.Api.Helpers
{
    /// <summary>
    /// Handles Tenant selection based on HTTP headers
    /// "X-Tenant-Name" = max_tenant.Organization Name
    /// "X-Tenant-Id" = max_tenant.TenantId
    /// </summary>
    public class TenantMiddleware
    {
        private readonly RequestDelegate next;

        public TenantMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context, ITenantService tenantService, IHttpContextAccessor httpContextAccessor)
        {
            //TODO: AKS : Needs to put the tenantlist in caching server so that no db call is made on each request
            //            Cache can be refreshed every 15 minutes so that newly added tenant can become active

            string tenantName = context.Request.Headers["X-Tenant-Name"].FirstOrDefault();
            if (string.IsNullOrEmpty(tenantName))
            {
                var tenantId = context.Request.Headers["X-Tenant-Id"].FirstOrDefault();
                if (string.IsNullOrEmpty(tenantId))
                {
                    await next.Invoke(context);
                }

                if(tenantId != null)
                {
                    ITenantService _tenantService = tenantService;
                    var tenant = await _tenantService.GetTenantById(tenantId);
                    context.Items["Tenant"] = tenant;
                }
            }
            else
            {

                if (tenantName != null)
                {
                    ITenantService _tenantService = tenantService;
                    var tenant = await _tenantService.GetTenantByOrganizationName(tenantName);
                    context.Items["Tenant"] = tenant;
                }
            }
            await next.Invoke(context);
        }
    }

    public static class TenantMiddlewareExtensions
    {
        public static IApplicationBuilder UseTenantMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TenantMiddleware>();
        }
    }
}
