using MediatR;
using Serilog;
using Serilog.Context;

namespace POSSystem.Infrastructure.Behaviors;

/// <summary>
/// Logging behavior for MediatR pipeline.
/// Follows cross-cutting concerns and observability patterns.
/// </summary>
/// <typeparam name="TRequest">The request type</typeparam>
/// <typeparam name="TResponse">The response type</typeparam>
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger _logger;

    public LoggingBehavior(ILogger logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var correlationId = Guid.NewGuid().ToString();

        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            _logger.Information("Executing request {RequestName} with correlation id {CorrelationId}", requestName, correlationId);

            try
            {
                var response = await next();
                _logger.Information("Executed request {RequestName} successfully", requestName);
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Exception executing request {RequestName}", requestName);
                throw;
            }
        }
    }
}


