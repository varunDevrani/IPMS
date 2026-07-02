using System.Security.Claims;
using IPMS.Data;
using IPMS.Entities;

namespace IPMS.Middlewares;


public class SessionValidationMiddleware
{
    private readonly RequestDelegate _next;
    
    public SessionValidationMiddleware(RequestDelegate next)
    {
        _next = next;
    } 

    public async Task InvokeAsync(HttpContext httpContext, AppDbContext dbContext)
    {
        if(httpContext.User.Identity?.IsAuthenticated == true)
        {
            string sid = httpContext.User.FindFirst(ClaimTypes.Sid)!.Value;

            TokenFamily? family = dbContext.TokenFamilies.FirstOrDefault(tf => tf.Id == Guid.Parse(sid));

            if(family == null || family.RevokedAt != null)
            {
                httpContext.Response.StatusCode = 401;
                return;
            }
        }
        await _next(httpContext);
    }
}