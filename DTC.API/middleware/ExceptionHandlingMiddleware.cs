using DTC.Application.ErrorHandlers;
using System.Net;
using System.Text.Json;

namespace DTC.API.middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (HttpExeption httpEx)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = httpEx.StatusCode;

                var response = new
                {
                    StatusCode = httpEx.StatusCode,
                    Message = httpEx.Message
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
            catch (Exception ex)
            {
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

                var response = new
                {
                    StatusCode = 500,
                    Message = "Произошла внутренняя ошибка сервера"
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        }
    }
}
