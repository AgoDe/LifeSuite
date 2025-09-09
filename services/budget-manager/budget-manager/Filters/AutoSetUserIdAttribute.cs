using budget_manager.Models;
using BudgetManager.Data.Models.Dto.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace budget_manager.Filters
{
    public class AutoSetUserIdAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {

            // Leggi l'userId dall'header (es. x-user-id)
            var userIdHeader = context.HttpContext.Request.Headers["x-user-id"].FirstOrDefault();

            if (string.IsNullOrEmpty(userIdHeader))
            {
                // Header mancante, puoi anche gestire un errore qui se vuoi
                base.OnActionExecuting(context);
                return;
            }

            foreach (var parameter in context.ActionArguments.Values)
            {
                // Caso 1: DTO con string UserId
                if (parameter is IUserOwnedFormDto userOwnedDtoString && userOwnedDtoString.UserId is string)
                {
                    userOwnedDtoString.UserId = userIdHeader;
                }
                // Caso 2: DTO con int UserId
                else if (parameter is IUserOwnedFormDto userOwnedDtoInt && userOwnedDtoInt.UserId is int)
                {
                    if (int.TryParse(userIdHeader, out var userIdInt))
                        userOwnedDtoInt.UserId = userIdInt;
                }
                // Caso 3: Qualsiasi altro tipo (es. filter)
                else if (parameter is IUserOwnedFilter userOwnedFilter)
                {
                    // Adatta qui a seconda del tipo di UserId nel filtro
                    if (userOwnedFilter.UserId is string)
                        userOwnedFilter.UserId = userIdHeader;
                    else if (userOwnedFilter.UserId is int && int.TryParse(userIdHeader, out var userIdInt))
                        userOwnedFilter.UserId = userIdInt;
                }
            }

            base.OnActionExecuting(context);
        }
    }
}
