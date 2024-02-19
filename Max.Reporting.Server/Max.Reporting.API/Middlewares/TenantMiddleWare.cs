using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace Max.Reporting.API.Middlewares
{
    public class TenantMiddleWare
    {
        private readonly RequestDelegate next;
        private readonly IMemoryCache _memoryCache;

        public TenantMiddleWare(RequestDelegate next, IMemoryCache memoryCache)
        {
            this.next = next;
            _memoryCache = memoryCache;
        }
        public async Task Invoke(HttpContext context, IHttpContextAccessor httpContextAccessor)
        {
            //TODO: AKS : Needs to put the tenantlist in caching server so that no db call is made on each request
            //            Cache can be refreshed every 15 minutes so that newly added tenant can become active

            var tenantId = context.Request.Headers["X-Tenant-Id"].FirstOrDefault();
            if (tenantId != null)
            {
                var catchTenantId = _memoryCache.Get<string>("TenantId");
                if (string.IsNullOrEmpty(catchTenantId))
                {
                    var tenantCN = context.Request.Headers["X-Tenant-CN"].FirstOrDefault();
                    var tenantRCN = context.Request.Headers["X-Tenant-RCN"].FirstOrDefault();
                    var cacheOptions = new MemoryCacheEntryOptions()
                         .SetAbsoluteExpiration(TimeSpan.FromMinutes(15));
                    _memoryCache.Set("TenantId", tenantId, cacheOptions);

                    var catchTenantCN = _memoryCache.Get<string>($"CN-{tenantId}");
                    if (string.IsNullOrEmpty(catchTenantCN))
                    {
                        _memoryCache.Set($"CN-{tenantId}", tenantCN, new MemoryCacheEntryOptions()
                         .SetAbsoluteExpiration(TimeSpan.FromHours(24)));
                    }
                    var catchTenantRCN = _memoryCache.Get<string>($"RCN-{tenantId}");
                    if(string.IsNullOrEmpty(catchTenantRCN))
                    {
                        _memoryCache.Set($"RCN-{tenantId}", tenantRCN, new MemoryCacheEntryOptions()
                         .SetAbsoluteExpiration(TimeSpan.FromHours(24)));
                    }
                    
                }
            }
            await next.Invoke(context);
        }
    }
}
