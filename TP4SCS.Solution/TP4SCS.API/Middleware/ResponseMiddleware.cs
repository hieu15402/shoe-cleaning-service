using System.Text.Json;

namespace TP4SCS.API.Middleware
{
    public class ResponseMiddleware
    {
        private readonly RequestDelegate _next;

        public ResponseMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await _next(context);

            // Check if the response status is 403
            if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
            {
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonSerializer.Serialize(new
                {
                    status = "error",
                    statusCode = 403,
                    message = "Truy Cập Bị Từ Chối!"
                }));
            }

            // Check if the response status is 401
            if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
            {
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonSerializer.Serialize(new
                {
                    status = "error",
                    statusCode = 401,
                    message = "Yêu Cầu Xác Thực!"
                }));
            }
        }
    }

}
