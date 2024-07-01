using System.Net;

namespace NZWalksAPI.Middlewares
{
    public class ExceptionHandlerMiddleware
    {
        private readonly ILogger logger;
        private readonly RequestDelegate next;

        public ExceptionHandlerMiddleware(ILogger logger, RequestDelegate next)
        {
            this.logger = logger;
            this.next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await next(httpContext);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.InnerException + "\n\nStackTrace: " + ex.StackTrace);

                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                httpContext.Response.ContentType = "application/Json";
                await httpContext.Response.WriteAsJsonAsync("Something went wrong!!!");
            }
        }
    }
}
