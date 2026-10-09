using POSSystem.Domain.Application.DTOs;
using MediatR;

namespace POSSystem.Services.Inventory.Application.Queries;

/// <summary>
/// CQRS Query for checking inventory availability.
/// </summary>
public record CheckInventoryAvailabilityQuery(Guid ProductId, int RequestedQuantity) : IRequest<InventoryAvailabilityDto>;

public record InventoryAvailabilityDto(
    Guid ProductId,
    bool IsAvailable,
    int AvailableQuantity,
    string Message);


