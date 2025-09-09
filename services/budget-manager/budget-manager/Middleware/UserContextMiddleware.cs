using budget_manager.Models;

namespace budget_manager.Middleware
{
    public class UserContextMiddleware
    {
        private readonly RequestDelegate _next;

        public UserContextMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, UserContext userContext)
        {
            if (context.Request.Headers.TryGetValue("x-user-id", out var userIdHeader))
            {
                userContext.UserId = userIdHeader.ToString();
            }
            await _next(context);   
        }
    }
}
