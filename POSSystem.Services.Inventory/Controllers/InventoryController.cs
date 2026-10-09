using POSSystem.Services.Inventory.Application.Commands;
using POSSystem.Services.Inventory.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace POSSystem.Services.Inventory.Controllers;

/// <summary>
/// API controller for inventory operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    private readonly IMediator _mediator;

    public InventoryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("check-availability")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<InventoryAvailabilityDto>> CheckAvailability(
        [FromBody] CheckInventoryAvailabilityQuery query,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(result);
    }

    [HttpPost("reserve")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<StockReservationDto>> ReserveStock(
        [FromBody] ReserveStockCommand command,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(command, cancellationToken);
        return Ok(result);
    }
}


