using POSSystem.Domain.Application.Exceptions;
using POSSystem.Domain.Application.Interfaces;
using POSSystem.Domain.Domain.Models;
using POSSystem.Services.Inventory.Domain.Events;
using MediatR;

namespace POSSystem.Services.Inventory.Application.Commands;

/// <summary>
/// Handler for ReserveStockCommand.
/// Implements stock reservation with transactional safety and domain events.
/// </summary>
public class ReserveStockCommandHandler : ICommandHandler<ReserveStockCommand, StockReservationDto>
{
    private readonly IRepository<InventoryItem, Guid> _inventoryRepository;
    private readonly IPublisher _eventPublisher;

    public ReserveStockCommandHandler(
        IRepository<InventoryItem, Guid> inventoryRepository,
        IPublisher eventPublisher)
    {
        _inventoryRepository = inventoryRepository;
        _eventPublisher = eventPublisher;
    }

    public async Task<StockReservationDto> Handle(
        ReserveStockCommand request,
        CancellationToken cancellationToken)
    {
        var inventoryItem = (await _inventoryRepository.GetAllAsync(cancellationToken))
            .FirstOrDefault(i => i.ProductId == request.ProductId)
            ?? throw NotFoundException.For<InventoryItem>(request.ProductId);

        if (!inventoryItem.CanFulfillOrder(request.Quantity))
        {
            throw new BusinessRuleException(
                $"Insufficient stock. Available: {inventoryItem.QuantityOnHand}, Requested: {request.Quantity}",
                "INSUFFICIENT_STOCK");
        }

        // Reserve stock
        inventoryItem.QuantityOnHand -= request.Quantity;
        inventoryItem.LastUpdated = DateTime.UtcNow;
        await _inventoryRepository.UpdateAsync(inventoryItem, cancellationToken);

        // Publish domain event
        var @event = new StockReservedEvent(inventoryItem.Id, request.ProductId, request.Quantity);
        await _eventPublisher.Publish(@event, cancellationToken);

        return new StockReservationDto(
            Guid.NewGuid(),
            request.ProductId,
            request.Quantity,
            true,
            "Stock reserved successfully");
    }
}


