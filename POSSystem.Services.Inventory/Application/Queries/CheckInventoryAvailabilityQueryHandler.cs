using POSSystem.Domain.Application.Exceptions;
using POSSystem.Domain.Application.Interfaces;
using POSSystem.Domain.Domain.Models;
using POSSystem.Services.Inventory.Domain.Events;
using MediatR;

namespace POSSystem.Services.Inventory.Application.Queries;

/// <summary>
/// Handler for CheckInventoryAvailabilityQuery.
/// Implements business logic for stock availability checking with domain events.
/// </summary>
public class CheckInventoryAvailabilityQueryHandler : IQueryHandler<CheckInventoryAvailabilityQuery, InventoryAvailabilityDto>
{
    private readonly IRepository<InventoryItem, Guid> _inventoryRepository;
    private readonly IPublisher _eventPublisher;

    public CheckInventoryAvailabilityQueryHandler(
        IRepository<InventoryItem, Guid> inventoryRepository,
        IPublisher eventPublisher)
    {
        _inventoryRepository = inventoryRepository;
        _eventPublisher = eventPublisher;
    }

    public async Task<InventoryAvailabilityDto> Handle(
        CheckInventoryAvailabilityQuery request,
        CancellationToken cancellationToken)
    {
        var inventoryItem = (await _inventoryRepository.GetAllAsync(cancellationToken))
            .FirstOrDefault(i => i.ProductId == request.ProductId)
            ?? throw NotFoundException.For<InventoryItem>(request.ProductId);

        var isAvailable = inventoryItem.CanFulfillOrder(request.RequestedQuantity);

        // Publish domain event
        var @event = isAvailable
            ? new StockCheckedEvent(inventoryItem.Id, request.ProductId, request.RequestedQuantity, true)
            : (DomainEvent)new InsufficientStockEvent(
                inventoryItem.Id,
                request.ProductId,
                inventoryItem.QuantityOnHand,
                request.RequestedQuantity);

        await _eventPublisher.Publish(@event, cancellationToken);

        return new InventoryAvailabilityDto(
            request.ProductId,
            isAvailable,
            inventoryItem.QuantityOnHand,
            isAvailable ? "Stock available" : "Insufficient stock");
    }
}


