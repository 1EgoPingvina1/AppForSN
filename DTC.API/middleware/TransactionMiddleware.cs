using DTC.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DTC.API.Middleware
{
    public class TransactionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<TransactionMiddleware> _logger;

        public TransactionMiddleware(RequestDelegate next, ILogger<TransactionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext, IUnitOfWork unitOfWork)
        {
            if (httpContext.Request.Method == HttpMethods.Get)
            {
                _logger.LogDebug("Transaction skipped for GET request: {Path}", httpContext.Request.Path);
                await _next(httpContext);
                return;
            }

            var strategy = unitOfWork.GetExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                await unitOfWork.BeginTransactionAsync();
                try
                {
                    _logger.LogInformation("Transaction started for {Path}", httpContext.Request.Path);

                    await _next(httpContext);

                    if (httpContext.Response.StatusCode >= 200 && httpContext.Response.StatusCode < 300)
                    {
                        await unitOfWork.CommitTransactionAsync();
                        _logger.LogInformation("Transaction committed for {Path}", httpContext.Request.Path);
                    }
                    else
                    {
                        await unitOfWork.RollbackAsync();
                        _logger.LogWarning("Transaction rolled back due to status code {StatusCode} for {Path}",
                            httpContext.Response.StatusCode, httpContext.Request.Path);
                    }
                }
                catch (Exception ex)
                {
                    await unitOfWork.RollbackAsync();
                    _logger.LogError(ex, "Transaction rolled back due to exception for {Path}", httpContext.Request.Path);
                    throw;
                }
            });
        }
    }
}
