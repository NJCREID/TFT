using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TFT_API.Filters
{
    public class ValidateRequestBodyAttribute : ActionFilterAttribute
    {
        // Override the method that executes before the action method is called
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Check if any action argument is null
            if (context.ActionArguments.Values.Any(arg => arg == null))
            {
                // Set the result to a bad request if the request body is missing
                context.Result = new BadRequestObjectResult("Request body is required.");
                return;
            }

            // Check if the model state is valid
            if (!context.ModelState.IsValid)
            {
                // Set the result to a bad request with validation error messages
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
