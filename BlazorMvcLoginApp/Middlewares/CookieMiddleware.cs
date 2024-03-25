namespace BlazorMvcLoginApp.Middlewares
{
    public class CookieMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (context.Request.Path.ToString().ToLower() == "/authentication/login")
            {
                await next.Invoke(context);
                return;
            }

            if (context.User.Identity is { IsAuthenticated: false })
            {
                context.Response.Redirect("/authentication/login");
                await next.Invoke(context);
                return;
            }

            await next.Invoke(context);
        }
    }
}
