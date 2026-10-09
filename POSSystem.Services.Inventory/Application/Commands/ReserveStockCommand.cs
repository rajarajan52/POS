using POSSystem.Domain.Application.DTOs;
using MediatR;

namespace POSSystem.Services.Inventory.Application.Commands;

/// <summary>
/// CQRS Command for reserving inventory stock.
/// </summary>
public record ReserveStockCommand(
    Guid ProductId,
    int Quantity) : IRequest<StockReservationDto>;

public record StockReservationDto(
    Guid ReservationId,
    Guid ProductId,
    int ReservedQuantity,
    bool Success,
    string Message);


