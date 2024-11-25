using Microsoft.AspNetCore.Http;
using Microsoft.Identity.Web;
using System.Security.Claims;

namespace pms_backend.Auth
{
    public static class UserInfo
    {
        public static string GetUserId(HttpContext context)
        {
            return context.User.GetObjectId();
        }

        public static string GetEmail(HttpContext context)
        {
            return context.User.FindFirst(ClaimTypes.Email)?.Value;
        }

        public static string GetName(HttpContext context)
        {
            return context.User.FindFirst(ClaimTypes.Name)?.Value;
        }
    }
}
