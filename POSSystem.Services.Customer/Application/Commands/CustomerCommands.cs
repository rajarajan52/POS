using POSSystem.Domain.Application.DTOs;
using POSSystem.Domain.Domain.Models;
using MediatR;

namespace POSSystem.Services.Customer.Application.Commands;

/// <summary>
/// CQRS Command for creating a new customer.
/// </summary>
public record CreateCustomerCommand(
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber) : IRequest<CustomerDto>;

/// <summary>
/// CQRS Command for adding loyalty points to a customer.
/// </summary>
public record AddLoyaltyPointsCommand(
    Guid CustomerId,
    decimal Points) : IRequest<CustomerDto>;


