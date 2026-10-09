using POSSystem.Domain.Application.DTOs;
using POSSystem.Domain.Application.Interfaces;
using CustomerModel = POSSystem.Domain.Domain.Models.Customer;
using POSSystem.Domain.Domain.Models;
using AutoMapper;
using MediatR;

namespace POSSystem.Services.Customer.Application.Queries;

/// <summary>
/// CQRS Query for retrieving a customer by ID.
/// </summary>
public record GetCustomerByIdQuery(Guid CustomerId) : IRequest<CustomerDto?>;

/// <summary>
/// Handler for GetCustomerByIdQuery.
/// </summary>
public class GetCustomerByIdQueryHandler : IQueryHandler<GetCustomerByIdQuery, CustomerDto?>
{
    private readonly IRepository<CustomerModel, Guid> _customerRepository;
    private readonly IMapper _mapper;

    public GetCustomerByIdQueryHandler(IRepository<CustomerModel, Guid> customerRepository, IMapper mapper)
    {
        _customerRepository = customerRepository;
        _mapper = mapper;
    }

    public async Task<CustomerDto?> Handle(GetCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(request.CustomerId, cancellationToken);
        return customer is null ? null : _mapper.Map<CustomerDto>(customer);
    }
}


