using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TFT_API.Filters
{
    public class ValidateRequestBodyAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ActionArguments.Values.Any(arg => arg == null))
            {
                context.Result = new BadRequestObjectResult("Request body is required.");
                return;
            }

            if (!context.ModelState.IsValid)
            {
                context.Result = new BadRequestObjectResult(new
                {
                    Message = "Validation failed",
                    Errors = context.ModelState
                        .Where(ms => ms.Value != null && ms.Value.Errors.Any())
                        .ToDictionary(
                            ms => ms.Key,
                            ms => ms.Value?.Errors.Select(e => e.ErrorMessage).ToArray() ?? []
                        )
                });
            }
        }
    }
}
