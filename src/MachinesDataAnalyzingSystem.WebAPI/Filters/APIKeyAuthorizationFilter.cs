using MachinesDataAnalyzingSystem.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MachinesDataAnalyzingSystem.WebAPI.Filters;

public class APIKeyAuthorizationFilter(IConfiguration configuration) : IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        if(!context.HttpContext.Request.Headers.ContainsKey(GlobalConstants.API_KEY_HEADER_NAME))
        {
            context.Result = new UnauthorizedObjectResult("API Key is missing.");
            return;
        }
        else if(context.HttpContext.Request.Headers[GlobalConstants.API_KEY_HEADER_NAME] != configuration["APIKey"])
        {
            context.Result = new UnauthorizedObjectResult("API Key is invalid.");
            return;
        }
    }
}