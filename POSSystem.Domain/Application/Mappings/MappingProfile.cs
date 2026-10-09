using POSSystem.Domain.Application.DTOs;
using POSSystem.Domain.Domain.Models;
using AutoMapper;

namespace POSSystem.Domain.Application.Mappings;

/// <summary>
/// AutoMapper profile for domain-to-DTO and DTO-to-domain mappings.
/// Centralizes mapping configuration following DRY principle.
/// </summary>
public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Product mappings
        CreateMap<Product, ProductDto>().ReverseMap();

        // Customer mappings
        CreateMap<Customer, CustomerDto>().ReverseMap();

        // Order mappings
        CreateMap<Order, OrderDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
        CreateMap<OrderLineItem, OrderLineItemDto>().ReverseMap();

        // Payment mappings
        CreateMap<Payment, PaymentDto>()
            .ForMember(dest => dest.Method, opt => opt.MapFrom(src => src.Method.ToString()))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
    }
}


