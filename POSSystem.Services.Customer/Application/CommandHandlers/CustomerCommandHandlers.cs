using POSSystem.Domain.Application.DTOs;
using POSSystem.Domain.Application.Exceptions;
using POSSystem.Domain.Application.Interfaces;
using CustomerModel = POSSystem.Domain.Domain.Models.Customer;
using POSSystem.Domain.Domain.Models;
using POSSystem.Services.Customer.Application.Commands;
using POSSystem.Services.Customer.Application.Services;
using AutoMapper;
using MediatR;

namespace POSSystem.Services.Customer.Application.CommandHandlers;

/// <summary>
/// Handler for CreateCustomerCommand.
/// </summary>
public class CreateCustomerCommandHandler : ICommandHandler<CreateCustomerCommand, CustomerDto>
{
    private readonly IRepository<CustomerModel, Guid> _customerRepository;
    private readonly IMapper _mapper;

    public CreateCustomerCommandHandler(IRepository<CustomerModel, Guid> customerRepository, IMapper mapper)
    {
        _customerRepository = customerRepository;
        _mapper = mapper;
    }

    public async Task<CustomerDto> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = new CustomerModel
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            LoyaltyPoints = 0
        };

        var createdCustomer = await _customerRepository.AddAsync(customer, cancellationToken);
        return _mapper.Map<CustomerDto>(createdCustomer);
    }
}

/// <summary>
/// Handler for AddLoyaltyPointsCommand.
/// </summary>
public class AddLoyaltyPointsCommandHandler : ICommandHandler<AddLoyaltyPointsCommand, CustomerDto>
{
    private readonly IRepository<CustomerModel, Guid> _customerRepository;
    private readonly LoyaltyProgramService _loyaltyService;
    private readonly IMapper _mapper;

    public AddLoyaltyPointsCommandHandler(
        IRepository<CustomerModel, Guid> customerRepository,
        LoyaltyProgramService loyaltyService,
        IMapper mapper)
    {
        _customerRepository = customerRepository;
        _loyaltyService = loyaltyService;
        _mapper = mapper;
    }

    public async Task<CustomerDto> Handle(AddLoyaltyPointsCommand request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(request.CustomerId, cancellationToken)
            ?? throw NotFoundException.For<CustomerModel>(request.CustomerId);

        _loyaltyService.AddPoints(customer, request.Points);
        var updatedCustomer = await _customerRepository.UpdateAsync(customer, cancellationToken);
        return _mapper.Map<CustomerDto>(updatedCustomer);
    }
}


