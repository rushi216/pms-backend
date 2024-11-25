using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Web;
using System.Threading.Tasks;
using System.Security.Claims;

namespace pms_backend.Auth
{
    public class TokenValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public TokenValidationMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Token will be automatically validated by Microsoft.Identity.Web middleware
            // Here we can add additional custom validation if needed
            if (context.User.Identity.IsAuthenticated)
            {
                // You can access claims here
                var userId = context.User.GetObjectId(); // This gets the user's Object ID (AAD user ID)
                context.Items["UserId"] = userId;
            }

            await _next(context);
        }
    }

    public static class TokenValidationMiddlewareExtensions
    {
        public static IApplicationBuilder UseTokenValidation(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TokenValidationMiddleware>();
        }
    }
}
