using POSSystem.Domain.Application.DTOs;
using POSSystem.Domain.Application.Interfaces;
using PaymentModel = POSSystem.Domain.Domain.Models.Payment;
using OrderModel = POSSystem.Domain.Domain.Models.Order;
using POSSystem.Domain.Domain.Models;
using POSSystem.Services.Payment.Application.Commands;
using POSSystem.Services.Payment.Application.Interfaces;
using POSSystem.Services.Payment.Application.Services;
using AutoMapper;
using MediatR;
using Serilog;
using ILogger = Serilog.ILogger;

namespace POSSystem.Services.Payment.Application.CommandHandlers;

/// <summary>
/// Handler for ProcessPaymentCommand.
/// Implements resilient payment processing with retry and circuit breaker patterns via Polly.
/// </summary>
public class ProcessPaymentCommandHandler : ICommandHandler<ProcessPaymentCommand, PaymentDto>
{
    private readonly IRepository<PaymentModel, Guid> _paymentRepository;
    private readonly IPaymentGateway _paymentGateway;
    private readonly ReceiptGenerationService _receiptService;
    private readonly IMapper _mapper;
    private readonly ILogger _logger;

    public ProcessPaymentCommandHandler(
        IRepository<PaymentModel, Guid> paymentRepository,
        IPaymentGateway paymentGateway,
        ReceiptGenerationService receiptService,
        IMapper mapper,
        ILogger logger)
    {
        _paymentRepository = paymentRepository;
        _paymentGateway = paymentGateway;
        _receiptService = receiptService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PaymentDto> Handle(ProcessPaymentCommand request, CancellationToken cancellationToken)
    {
        _logger.Information("Processing payment for OrderId={OrderId}, Amount={Amount}", request.OrderId, request.Amount);

        // Process payment through gateway (resilience handled by Polly in middleware)
        var paymentRequest = new PaymentRequest
        {
            OrderId = request.OrderId,
            Amount = request.Amount,
            PaymentMethod = request.Method.ToString(),
            CardToken = request.CardNumber
        };

        var paymentResult = await _paymentGateway.ProcessPaymentAsync(
            paymentRequest,
            cancellationToken);

        var payment = new PaymentModel
        {
            OrderId = request.OrderId,
            Amount = request.Amount,
            Method = request.Method,
            Status = paymentResult.IsSuccessful ? PaymentStatus.Completed : PaymentStatus.Failed,
            TransactionReference = paymentResult.TransactionId,
            ProcessedAt = DateTime.UtcNow,
            ReceiptNumber = _receiptService.GenerateReceiptNumber()
        };

        var savedPayment = await _paymentRepository.AddAsync(payment, cancellationToken);

        if (paymentResult.IsSuccessful)
        {
            _logger.Information(
                "Payment processed successfully: OrderId={OrderId}, TransactionRef={TransactionRef}",
                request.OrderId,
                paymentResult.TransactionId);
        }
        else
        {
            _logger.Warning(
                "Payment processing failed: OrderId={OrderId}, Message={Message}",
                request.OrderId,
                paymentResult.Message);
        }

        return _mapper.Map<PaymentDto>(savedPayment);
    }
}


