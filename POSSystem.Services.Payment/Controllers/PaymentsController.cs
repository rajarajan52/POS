using POSSystem.Domain.Application.DTOs;
using POSSystem.Services.Payment.Application.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace POSSystem.Services.Payment.Controllers;

/// <summary>
/// API controller for payment operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PaymentsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("process")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PaymentDto>> ProcessPayment(
        [FromBody] ProcessPaymentCommand command,
        CancellationToken cancellationToken)
    {
        var payment = await _mediator.Send(command, cancellationToken);
        return Ok(payment);
    }
}


