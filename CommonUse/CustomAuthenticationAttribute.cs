using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace CommonUse
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class CustomAuthenticationAttribute : Attribute, IAuthorizationFilter
    {
        public string Roles { get; set; }
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var headers = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            var token = headers?.Split(" ").Last();
            if (token == null)
            {
                context.Result = new JsonResult(

                    new { message = "You are not authortized to use this API" })
                {
                    StatusCode = StatusCodes.Status401Unauthorized
                };
            }
            else
            {
                var validator = context.HttpContext.RequestServices.GetRequiredService<TokenValidator>();
                var user = validator.Validate(token).Result;
                if (user == null)
                {
                    context.Result = new JsonResult(new { message = "You are not authortized to use this API" })
                    {
                        StatusCode = StatusCodes.Status401Unauthorized
                    };
                }
                else
                {

                    var roles = Roles.Split(",").ToList();
                    var roleExists = roles.Exists(c => c == user.Role.RoleName);
                    if (!roleExists)
                    {
                        context.Result = new JsonResult(new { message = "You are not authortized to use this API" })
                        {
                            StatusCode = StatusCodes.Status401Unauthorized
                        };
                    }

                }
            }
        }
    }
}
