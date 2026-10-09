using POSSystem.Services.Inventory.Domain.Events;
using MediatR;
using Serilog;
using Microsoft.Extensions.Logging;
using ILogger = Serilog.ILogger;

namespace POSSystem.Services.Inventory.Application.EventHandlers;

/// <summary>
/// Event handler for StockReservedEvent.
/// Implements event-driven architecture for async processing and notifications.
/// </summary>
public class StockReservedEventHandler : INotificationHandler<StockReservedEvent>
{
    private readonly ILogger _logger;

    public StockReservedEventHandler(ILogger logger)
    {
        _logger = logger;
    }

    public Task Handle(StockReservedEvent @event, CancellationToken cancellationToken)
    {
        _logger.Information(
            "Stock reserved: ProductId={ProductId}, Quantity={Quantity}, CorrelationId={CorrelationId}",
            @event.ProductId,
            @event.ProductId,
            @event.AggregateId);

        // TODO: Publish to message queue for downstream services (Order Service, etc.)
        return Task.CompletedTask;
    }
}

/// <summary>
/// Event handler for InsufficientStockEvent.
/// </summary>
public class InsufficientStockEventHandler : INotificationHandler<InsufficientStockEvent>
{
    private readonly ILogger _logger;

    public InsufficientStockEventHandler(ILogger logger)
    {
        _logger = logger;
    }

    public Task Handle(InsufficientStockEvent @event, CancellationToken cancellationToken)
    {
        _logger.Warning(
            "Insufficient stock alert: ProductId={ProductId}, Available={Available}, Requested={Requested}",
            @event.ProductId,
            @event.AvailableQuantity,
            @event.RequestedQuantity);

        // TODO: Send alert to inventory manager, trigger reorder if at reorder level
        return Task.CompletedTask;
    }
}


