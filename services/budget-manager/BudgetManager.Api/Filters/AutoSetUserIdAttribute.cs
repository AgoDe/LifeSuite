using BudgetManager.Api.Models;
using BudgetManager.Data.Models.Dto.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BudgetManager.Api.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true)]
    public class AutoSetUserIdAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {

            // Leggi l'userId dall'header (es. x-user-id)
            var userIdHeader = context.HttpContext.Request.Headers["x-user-id"].FirstOrDefault();

            if (string.IsNullOrEmpty(userIdHeader))
            {
                // Header mancante, puoi anche gestire un errore qui se vuoi
                context.Result = new UnauthorizedResult();
                base.OnActionExecuting(context);
                return;
            }

            foreach (var parameter in context.ActionArguments.Values)
            {
                // Caso 1: DTO con string UserId
                if (parameter is IUserOwnedFormDto userOwnedDtoString)
                {
                    userOwnedDtoString.UserId = userIdHeader;
                }

                // Caso 3: Qualsiasi altro tipo (es. filter)
                else if (parameter is IUserOwnedFilter userOwnedFilter)
                {
                    userOwnedFilter.UserId = userIdHeader;
                }
            }

            base.OnActionExecuting(context);
        }
    }
}
