using POSSystem.Domain.Application.DTOs;
using POSSystem.Services.Customer.Application.Commands;
using POSSystem.Services.Customer.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace POSSystem.Services.Customer.Controllers;

/// <summary>
/// API controller for customer operations.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly IMediator _mediator;

    public CustomersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CustomerDto>> CreateCustomer(
        [FromBody] CreateCustomerCommand command,
        CancellationToken cancellationToken)
    {
        var customer = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetCustomerById), new { id = customer.Id }, customer);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerDto>> GetCustomerById(Guid id, CancellationToken cancellationToken)
    {
        var customer = await _mediator.Send(new GetCustomerByIdQuery(id), cancellationToken);
        if (customer is null)
        {
            return NotFound();
        }

        return Ok(customer);
    }

    [HttpPost("{id}/loyalty/add-points")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerDto>> AddLoyaltyPoints(
        Guid id,
        [FromBody] AddLoyaltyPointsRequest request,
        CancellationToken cancellationToken)
    {
        var customer = await _mediator.Send(
            new AddLoyaltyPointsCommand(id, request.Points),
            cancellationToken);
        return Ok(customer);
    }
}

public record AddLoyaltyPointsRequest(decimal Points);


