using POSSystem.Domain.Application.DTOs;
using POSSystem.Domain.Domain.Models;
using MediatR;

namespace POSSystem.Services.Payment.Application.Commands;

/// <summary>
/// CQRS Command for processing a payment.
/// </summary>
public record ProcessPaymentCommand(
    Guid OrderId,
    decimal Amount,
    PaymentMethod Method,
    string CardNumber = "") : IRequest<PaymentDto>;


