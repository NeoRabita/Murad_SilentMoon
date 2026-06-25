using Microsoft.AspNetCore.Mvc.Filters;

namespace SilentMoon.WebApi.StartupInjections.Validations
{
    public class ValidateModelAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
          
        }
    }
}